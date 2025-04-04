# Grocery Inventory System

A .NET Core application for managing grocery inventory, suppliers, and orders.

## Features

- Manage grocery items
- Track suppliers
- Handle orders
- RESTful API endpoints

## Jenkins Integration

- Automated builds
- Continuous Integration
- Automated testing
- Deployment pipeline

## Technology Stack

- **Backend**: ASP.NET Core 7.0, Entity Framework Core
- **Frontend**: ASP.NET Core MVC, Bootstrap, JavaScript
- **Database**: PostgreSQL
- **DevOps**: Docker, GitHub Actions

## Getting Started

### Prerequisites

- [.NET SDK 7.0](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio Code](https://code.visualstudio.com/) with recommended extensions
- [Docker](https://www.docker.com/get-started) (optional)

### Setup

1. Clone the repository

```
git clone https://github.com/yourusername/GroceryInventory.git
cd GroceryInventory
```

2. Open the workspace in VS Code

```
code GroceryInventory.code-workspace
```

3. Install recommended VS Code extensions when prompted

4. Restore packages

```
dotnet restore
```

5. Update database connection string in `appsettings.json` files

6. Apply database migrations

```
dotnet ef database update --project src/GroceryInventory.Data/GroceryInventory.Data.csproj --startup-project src/GroceryInventory.API/GroceryInventory.API.csproj
```

7. Run the application

```
dotnet run --project src/GroceryInventory.Web/GroceryInventory.Web.csproj
```

## Project Structure

```
GroceryInventory/
│
├── src/                                  # Source code
│   ├── GroceryInventory.API/             # REST API
│   ├── GroceryInventory.Web/             # Web UI
│   ├── GroceryInventory.Core/            # Core business logic
│   ├── GroceryInventory.Data/            # Data access layer
│   └── GroceryInventory.Common/          # Shared utilities
│
├── tests/                                # Test projects
│   ├── GroceryInventory.UnitTests/
│   ├── GroceryInventory.IntegrationTests/
│   └── GroceryInventory.FunctionalTests/
│
├── db/                                   # Database setup and migration
│
├── infrastructure/                       # Infrastructure automation
│
├── docs/                                 # Documentation
│
└── tools/                                # Tools and utilities
```

## Development

### VS Code Tasks

The following tasks are available in VS Code (Ctrl+Shift+P → "Tasks: Run Task"):

- **build**: Build the entire solution
- **publish**: Prepare the application for deployment
- **watch**: Run the web application with hot reload
- **ef-migrations-add**: Add a new Entity Framework migration
- **ef-database-update**: Apply migrations to the database
- **test**: Run all tests in the solution

## License

This project is licensed under the MIT License - see the LICENSE file for details.
