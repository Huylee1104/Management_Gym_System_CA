# Gym Management System – Specification Constitution

## 1. Project Overview

This project is a **Gym Management System** designed to manage gym members, membership packages, product sales, inventory, and financial transactions.

The system supports two main user roles:

- Member (Gym customer)
- Manager (Gym staff / administrator)

The platform supports the following gym operations:

- Membership registration
- Membership suspension
- Selling membership packages
- Selling physical products
- Inventory management
- Financial transaction tracking
- Financial reporting

The system will be implemented as a **web application**.

---

# 2. System Roles

## 2.1 Member

Members are gym customers who purchase membership packages and may buy additional products.

### Capabilities

Members can:

- Register a membership package via the website
- Register a membership package directly at the gym
- Pay for membership packages
- Buy physical products from the gym
- Temporarily pause their membership
- Resume their membership later

### Important Rule

Membership suspension **cannot be performed online**.

The member must **inform the manager directly**.  
The manager will update the system to suspend or resume the membership.

### Member Actions

Members can perform the following actions:

- Purchase gym membership packages
- Purchase physical products
- Request membership suspension
- Request membership reactivation

---

## 2.2 Manager

Managers operate and maintain the gym management system.

### Responsibilities

Managers are responsible for:

- Registering new members when they register directly at the gym
- Assigning membership packages during registration
- Managing products and membership packages
- Managing inventory
- Managing financial transactions
- Generating financial reports

### Manager Capabilities

Managers can perform the following actions:

- Create new member accounts
- Assign gym membership packages to members
- Import products into inventory
- Export products from inventory
- Sell products to members
- Manage financial transactions
- Generate financial reports based on time period

---

# 3. Technology Stack

## 3.1 Frontend

The frontend will be developed using:

- HTML
- CSS
- JavaScript

### Frontend Libraries

The following libraries will be used:

- Bootstrap
- jQuery
- Tom-Select
- Toastr
- Bootstrap Icons

Frontend responsibilities include:

- User interface rendering
- Form submission
- User notifications
- Client-side interaction

---

## 3.2 Backend

The backend will be implemented using:

- **ASP.NET Core MVC 8**

### Backend Libraries

- Newtonsoft.Json
- QuestPDF
- Microsoft.Extensions.DependencyInjection
- Microsoft.EntityFrameworkCore (version 9.0.14)
- ClosedXML

Backend responsibilities include:

- Handling business logic
- Managing HTTP requests
- Processing financial transactions
- Generating reports
- Managing database communication

---
## 3.3 Database

### Development Database

For development and AI-assisted code generation, the system will temporarily use:

- **SQLite**

SQLite is chosen because:

- It requires no server installation
- It works well with Entity Framework Core
- It simplifies local development and testing
- It allows AI tools to generate and run the project easily

### Production Database

In production, the system is designed to use:

- **PostgreSQL**

The database provider can be switched from SQLite to PostgreSQL by updating:

- Entity Framework Core provider
- Connection string
- Database migrations

### ORM

- Entity Framework Core

### Stored Data

The database stores the following information:

- User data
- Membership information
- Product information
- Financial transactions
- Inventory data

## Frontend Libraries

The system uses the following frontend libraries stored in `wwwroot/libs`:

- Bootstrap – UI framework and responsive layout
- Bootstrap Icons – icon library
- jQuery – DOM manipulation and AJAX support
- Toastr – notification messages
- Tom Select – enhanced select dropdown UI

Compiled CSS files are stored in:

wwwroot/dist/css/

## Current Project Structure

Management_Gym_System/
 ├─ Controllers/
 ├─ Models/
 ├─ Repositories/
 ├─ Services/
 ├─ Views/
 ├─ wwwroot/
 │   ├─ dist/css/
 │   ├─ libs/
 │   │   ├─ bootstrap
 │   │   ├─ bootstrap-icons
 │   │   ├─ jquery
 │   │   ├─ toastr
 │   │   └─ tom-select
 │   └─ img/