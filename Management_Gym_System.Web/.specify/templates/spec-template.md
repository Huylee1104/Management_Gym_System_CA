# 4. Core Business Domains

The system consists of four main domains:

- User Management
- Product and Service Management
- Financial Management
- Inventory Management

---

# 5. User Management

## Table: UserRole

Purpose:

Defines system roles used for access control.

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK, Identity | Role identifier |
| RoleName | nvarchar(50) | Not Null | Role name (Member, Manager) |
| Status | bit | Not Null | 1 = Active, 0 = Disabled |

---

## Table: User

Purpose:

Stores information of all users in the system.

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK, Identity | User identifier |
| RoleID | bigint | FK | Reference to UserRole |
| FullName | nvarchar(100) | Not Null | User full name |
| Avatar | nvarchar(max) | Null | Avatar image path |
| PhoneNumber | varchar(15) | Null | Contact number |
| Status | bit | Not Null | 1 = Active, 0 = Disabled |

---

# 6. Product and Service Management

## Table: ProductCategory

Purpose:

Defines product categories.

Examples:

- Beverages
- Supplements
- Gym Packages

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK, Identity | Category ID |
| CategoryName | nvarchar(100) | Not Null | Category name |
| Status | bit | Not Null | 1 = Active, 0 = Inactive |

---

## Table: Product

Purpose:

Stores all sellable items including gym packages and physical products.

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK, Identity | Product ID |
| CategoryID | bigint | FK | Product category |
| ProductName | nvarchar(200) | Not Null | Product name |
| Price | decimal(18,2) | Not Null | Selling price |
| Unit | nvarchar(50) | Not Null | Unit (Bottle, Month, Package) |
| Status | bit | Not Null | 1 = Available, 0 = Discontinued |

---

# 7. Membership Management

## Table: GymMembershipCard

Purpose:

Represents a membership package purchased by a member.

A user may own multiple membership cards due to renewals.

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK, Identity | Membership card ID |
| UserID | bigint | FK | Owner of the membership |
| ProductID | bigint | FK | Purchased membership package |
| StartDate | date | Not Null | Start date |
| EndDate | date | Not Null | Expiration date |
| PauseDate | date | Null | Suspension start date |
| ResumeDate | date | Null | Resume date |
| Notes | nvarchar(500) | Null | Suspension reason |
| Status | bit | Not Null | 1 = Active, 0 = Expired |

---

# 8. Financial Management

## Table: FinancialTransaction

Purpose:

Stores all financial transactions.

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK, Identity | Transaction ID |
| CustomerID | bigint | FK | Member making payment |
| StaffID | bigint | FK | Staff processing transaction |
| TransactionDate | datetime | Not Null | Transaction timestamp |
| TransactionType | bit | Not Null | 1 = Income, 0 = Expense |
| TotalAmount | decimal(18,2) | Not Null | Total value |
| Note | nvarchar(500) | Null | Description |

---

## Table: TransactionDetail

Purpose:

Stores product details within a transaction.

| Column | Type | Constraints | Description |
|------|------|------|------|
| ID | bigint | PK | Detail ID |
| TransactionID | bigint | FK | Parent transaction |
| ProductID | bigint | FK | Purchased product |
| Quantity | int | Not Null | Quantity |
| UnitPrice | decimal(18,2) | Not Null | Price at purchase |
| SubTotal | decimal(18,2) | Not Null | Quantity × UnitPrice |

---

# 9. Inventory Management

## Table: ImportReceipt

Records product imports.

| Column | Type |
|------|------|
| ID | bigint (PK) |
| StaffID | bigint (FK) |
| TransactionID | bigint (FK, Nullable) |
| ImportDate | datetime |

---

## Table: ImportReceiptDetail

| Column | Type |
|------|------|
| ID | bigint (PK) |
| ImportReceiptID | bigint (FK) |
| ProductID | bigint (FK) |
| Quantity | int |
| ImportPrice | decimal(18,2) |

---

## Table: ExportReceipt

Records product exports.

| Column | Type |
|------|------|
| ID | bigint (PK) |
| StaffID | bigint (FK) |
| ExportDate | datetime |
| Note | nvarchar(500) |

---

## Table: ExportReceiptDetail

| Column | Type |
|------|------|
| ID | bigint (PK) |
| ExportReceiptID | bigint (FK) |
| ProductID | bigint (FK) |
| Quantity | int |
| ExportPrice | decimal(18,2) |

# 6. Database Relationships

## One-to-Many Relationships

### UserRole → User
One role can belong to multiple users.

### ProductCategory → Product
One category can contain multiple products.

### User → FinancialTransaction
A user may participate in multiple financial transactions.

### User → ImportReceipt
A staff member may create multiple import receipts.

### User → ExportReceipt
A staff member may create multiple export receipts.

### Product → TransactionDetail
A product may appear in multiple transaction details.

### Product → ImportReceiptDetail
A product may be imported multiple times.

### Product → ExportReceiptDetail
A product may be exported multiple times.

---

# 7. Master–Detail Relationships

## FinancialTransaction → TransactionDetail
One financial transaction may contain multiple purchased items.

## ImportReceipt → ImportReceiptDetail
One import receipt may include multiple imported products.

## ExportReceipt → ExportReceiptDetail
One export receipt may include multiple exported products.

---

# 8. Special Relationships

## GymMembershipCard → Product

Each membership card corresponds to **one membership package product**.

A membership product represents a service such as:

- 1 Month Gym Package
- 3 Month Gym Package
- 1 Year Gym Package

---

## ImportReceipt → FinancialTransaction (Optional)

An import receipt may optionally link to a **financial expense transaction**.

This allows the system to track the **payment made to suppliers** when inventory is imported.

---

# 9. Key Business Workflows

## 9.1 Member Registration

### Flow

1. A member registers online or directly at the gym.
2. The manager creates a user account if the member does not already exist.
3. The member selects a membership package.
4. The system records a financial transaction for the payment.
5. The system creates a GymMembershipCard associated with the member.

---

## 9.2 Membership Suspension

### Flow

1. The member requests membership suspension directly at the gym.
2. The manager updates the membership record.
3. The system updates the following fields:

- `PauseDate`
- `ResumeDate`

4. The system adjusts the membership validity period accordingly.

---

## 9.3 Product Sales

### Flow

1. The manager creates a financial transaction.
2. The manager adds purchased products to `TransactionDetail`.
3. The system calculates the `TotalAmount`.
4. The transaction is saved to the database.

---

## 9.4 Inventory Import

### Flow

1. The manager creates an `ImportReceipt`.
2. The manager adds products to `ImportReceiptDetail`.
3. The system updates inventory quantities.
4. An optional expense transaction may be created to record supplier payment.