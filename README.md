# Academic Library App

## Application Purpose and Group Members
This is a web application to manage the catalog and loans of an academic library. It allows users to browse books, borrow them, and track their active loans, while administrators can manage the library's inventory.
**Developed by:** Sergio Gonzalez and Arthur Chevreau.

## Live URL
[Live Application](https://lab2-library-app20261024151541-a5g4d7a3cxhxbrbp.swedencentral-01.azurewebsites.net/)

## Technology Stack
* **Framework:** .NET 10.0 (ASP.NET Core MVC)
* **Language:** C#
* **ORM:** Entity Framework Core
* **Identity:** ASP.NET Core Identity
* **Database:** SQL Server LocalDB (Development) / SQLite (Production)
* **Frontend:** Razor Views, HTML/CSS (Bootstrap)

## Instructions for Running Locally
1. Clone or extract the project repository.
2. Open a terminal in the project root folder.
3. Initialize user secrets for the Admin account:
   `dotnet user-secrets init`
   `dotnet user-secrets set "AdminPassword" "YourSecurePassword123!"`
4. Apply the database migrations to create the local database:
   `dotnet ef database update`
5. Run the application:
   `dotnet run`

## Configuration and Secure Admin Initialization
The application utilizes ASP.NET Core Identity for authentication and authorization. The "Admin" role and the initial system administrator account are securely generated at startup via the `SeedData.cs` class. 

The password for the Admin account is never hardcoded. In the local development environment, it is retrieved securely from `dotnet user-secrets` using the configuration key `AdminPassword`. In the deployed production environment (Azure), this same key is injected via the hosting platform's Environment Variables / Application Settings.

## Review Access (Test Accounts)
The database has been seeded with accounts to test multi-user interaction and role-based authorization. 
* **Available Roles:** Admin, User (two separate ordinary user accounts are available).
* **Note:** Passwords are not included in this repository. Login credentials (usernames and passwords) have been provided directly to the teacher and the assigned opposing group via a private Canvas message, as per the assignment instructions.

## Key Technical Choices
* **Server-Side Processing:** Book search and genre filtering are implemented purely on the server side using LINQ and Entity Framework Core, ensuring that only the required data is fetched from the database.
* **Role-Based UI:** Razor views dynamically hide administrative actions (like "Create New" or "Edit") and user-specific sections (like "Library Activity") based on the authenticated user's role to prevent interface clutter.
* **Business Logic:** Server-side validation prevents users from borrowing a book that is already on loan, maintaining data integrity without relying on client-side state.

## Deployment and Persistent Data
The application is deployed on Azure App Service. 
* **Data Storage:** The production database uses SQLite. The database file is stored persistently in the `HOME/data` directory on the Azure server, outside the published application files, ensuring it is not overwritten during deployments.
* **Persistence Check:** We have verified that data (users, books, and active loans) survives both application restarts and code redeployments.