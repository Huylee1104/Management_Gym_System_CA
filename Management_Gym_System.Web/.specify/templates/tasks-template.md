# 10. System Modules and Functional Specifications

This section defines the complete system modules, including business operations, API endpoints, validation rules, inputs, outputs, and test scenarios.

---

# 10.1 Category & Product Management Module

This module manages base data (master data) used by other modules in the system.

It includes:

- User roles
- Product categories
- Products
- Membership packages

---

## 10.1.1 Role Management

### Description

This feature manages the roles used for system authorization.

Roles determine what actions a user can perform.

Example roles:

- Admin
- Receptionist
- Trainer
- Member

---

### Functions

- Create role
- Update role
- View role list
- Activate / deactivate role

---

### API Endpoints

GET /api/roles  
POST /api/roles  
POST /api/roles/{id}  
POST /api/roles/{id}/status  

---

### Input Fields

| Field | Type | Required | Description |
|------|------|------|------|
| RoleName | string | Yes | Name of the role |
| Status | boolean | Yes | Active or inactive |

---

### Business Rules

- RoleName must be unique.
- Only **Admin users** can manage roles.
- Roles cannot be deleted if they are currently assigned to users.

---

### Output

List of roles stored in the database.

---

### Test Cases

1. Create a new role **Personal Trainer** → verify a new record appears in `UserRole`.
2. Update role name → verify changes are saved.
3. Disable a role → verify `Status = 0`.

---

# 10.1.2 Product Category Management

### Description

This feature manages categories used to classify products and services.

Examples:

- Drinks
- Supplements
- Gym Packages
- Accessories

---

### Functions

- Create category
- Update category
- List categories
- Change category status

---

### API Endpoints

GET /api/product-categories  
POST /api/product-categories  
POST /api/product-categories/{id}  
POST /api/product-categories/{id}/status  

---

### Input Fields

| Field | Type | Required |
|------|------|------|
| CategoryName | string | Yes |
| Status | boolean | Yes |

---

### Business Rules

- CategoryName must be unique.
- Categories cannot be deleted if products exist in the category.

---

### Test Case

Create category **Supplements** → verify category appears in category list.

---

# 10.1.3 Product & Service Management

### Description

This feature manages all sellable items in the gym.

Products include:

- Membership packages
- Drinks
- Gym accessories
- Supplements

---

### Functions

- Create product
- Update product
- List products
- Enable / disable product
- Search products

---

### API Endpoints

GET /api/products  
POST /api/products  
POST /api/products/{id}  
POST /api/products/{id}/status  

---

### Input Fields

| Field | Type | Required |
|------|------|------|
| ProductName | string | Yes |
| CategoryID | bigint | Yes |
| Price | decimal | Yes |
| Unit | string | Yes |
| Status | boolean | Yes |

---

### Business Rules

- CategoryID must exist in `ProductCategory`.
- Price must be greater than zero.
- Product name should be unique within a category.

---

### Output

Product list displayed in the POS sales interface.

---

### Test Case

Update price of **Gym 1 Year Package** from 5,000,000 to 4,000,000 → verify new invoices use updated price.

---

# 10.2 User & Member Management Module

This module manages all users and gym members.

---

## 10.2.1 User Profile Management

### Description

Stores and manages user information.

Users include:

- Admin
- Staff
- Trainers
- Members

---

### Functions

- Create user
- Update user
- View user list
- Deactivate user

---

### API Endpoints

GET /api/users  
POST /api/users  
POST /api/users/{id}  
POST /api/users/{id}/status  

---

### Input Fields

| Field | Type | Required |
|------|------|------|
| FullName | string | Yes |
| PhoneNumber | string | Yes |
| Avatar | string | No |
| RoleID | bigint | Yes |

---

### Business Rules

- PhoneNumber must be unique.
- RoleID must exist in `UserRole`.

---

### Validation Rules

- FullName cannot be empty.
- PhoneNumber must follow valid phone format.

---

### Test Case

Create user without `FullName` → system returns **400 Bad Request**.

---

# 10.2.2 Membership Card Operations

### Description

This module manages membership cards purchased by members.

Features include:

- Membership activation
- Membership renewal
- Membership suspension
- Membership reactivation

---

### API Endpoints

POST /api/membership/renew  
POST /api/membership/suspend  
POST /api/membership/reactivate  

---

### Input Fields

| Field | Type | Description |
|------|------|------|
| MembershipID | bigint | Membership card ID |
| ExtendMonths | int | Number of months to extend |
| PauseDate | date | Suspension start |
| ResumeDate | date | Suspension end |

---

### Business Rules

- Membership must belong to an active user.
- Suspension cannot be applied to expired cards.
- Renewal must extend `EndDate`.

---

### Output

Updated membership record.

---

### Test Case

Suspend expired membership → system rejects request.

---

# 10.3 Sales & Finance Module

---

## 10.3.1 Sales Invoice Creation

### Description

Records customer purchases including:

- Membership packages
- Products

---

### API Endpoint

POST /api/finance/create-invoice

---

### Input

| Field | Type |
|------|------|
| UserID | bigint |
| Items | List<ProductPurchase> |

ProductPurchase

| Field | Type |
|------|------|
| ProductID | bigint |
| Quantity | int |

---

### Business Rules

- If product type is **membership package**, a `GymMembershipCard` must be created.
- TotalAmount must equal sum of item subtotals.

---

### Output

- One `FinancialTransaction`
- Multiple `TransactionDetail`

---

### Test Case

Buy:

- 1 membership package
- 2 bottles of water

Verify total calculation.

---

## 10.3.2 Expense Management

### Description

Tracks gym expenses.

Examples:

- Equipment purchase
- Supplier payments
- Maintenance costs

---

### API Endpoint

POST /api/finance/create-expense

---

### Input

| Field | Type |
|------|------|
| RecipientID | bigint |
| Amount | decimal |
| Description | string |

---

### Business Rules

- Expense cannot exceed available balance.

---

# 10.4 Inventory Management Module

---

## 10.4.1 Inventory Import

### Description

Records incoming stock from suppliers.

---

### API Endpoint

POST /api/inventory/import

---

### Input

| Field | Type |
|------|------|
| StaffID | bigint |
| Items | List<ImportItem> |

---

### Output

Inventory quantity increases.

---

### Test Case

Import 100 bottles of water → inventory increases by 100.

---

## 10.4.2 Inventory Export

### Description

Records stock leaving the warehouse.

Reasons:

- Product sales
- Internal usage
- Damaged goods

---

### API Endpoint

POST /api/inventory/export

---

### Input

| Field | Type |
|------|------|
| StaffID | bigint |
| Items | List<ExportItem> |
| Reason | string |

---

### Business Rules

- Export quantity cannot exceed inventory.

---

# 10.5 Operations & Check-In Module

---

## 10.5.1 Member Check-In Verification

### Description

Validates whether a member is allowed to enter the gym.

---

### API Endpoint

GET /api/operations/check-in/{userId}

---

### Validation Conditions

Membership must satisfy:

- `EndDate >= Today`
- `Status = Active`
- `PauseDate IS NULL`

---

### Output

| Result | Meaning |
|------|------|
| True | Access granted |
| False | Access denied |

---

### Example

Member with suspended membership → return:

"Membership is currently suspended."

---

# 10.6 Reporting & Analytics Module

---

## 10.6.1 Revenue Report

### API Endpoint

GET /api/reports/revenue?fromDate=&toDate=

---

### Output

- Total Income
- Total Expense
- Net Profit

---

## 10.6.2 Member Traffic Report

### API Endpoint

GET /api/reports/member-traffic

---

### Output

Chart showing number of check-ins by time interval.

---

## 10.6.3 Inventory Report

### API Endpoint

GET /api/reports/inventory

---

### Inventory Calculation

Inventory =

Total Imported  
− Total Exported  
− Total Sold