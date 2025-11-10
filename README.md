# Jet Interior Design System (JIDS)
A modular WPF application that simulates the design of commercial jet interiors. Built using C# and the MVVM pattern, the system allows users to create, configure and manage multiple jet layouts with tier specific features and accessibility options.

## Features
- Create and manage multiple jet configurations
- Tier specific settings (Economy, Business)
- Accessibility options and Wi-Fi/display controls
- JSON based save and load functionality
- MahApps.Metro UI theming

## Installation
1. Clone the repository:
``` bash
git clone https://github.com/Alex-Cooke192/Object_Oriented_Programming_Assignment_2_HIA.git 
```
2. Open the solution in Visual Studio 2022 or later.
3. Restore NuGet packages
    - Open the Package Manager Console and run:
``` bash
	Update-Package -reinstall
```
4. Build and run the project.

## Usage
- Log in with your user credentials
- Create a new configuration or load an existing one
- Use the layout editor to add and configure components
- Save your configuration to JSON for future use

## Project Structure
- Views: XAML UI files
- ViewModels: MVVM logic and commands
- Models: Domain entities
- Services: Business logic
- Repositories: JSON persistence
- Migrations: Versioning or transformation logic for configuration formats
- Data: Static resources
- Interfaces: Abstractions for services and repositories
- Tests: Unit and integration tests

## Dependencies
- .NET 6 or later (https://dotnet.microsoft.com/)
- MahApps.Metro (https://github.com/MahApps/MahApps.Metro)
- BCrypt.Net-Next (https://www.nuget.org/packages/BCrypt.Net-Next/)
- Microsoft.EntityFrameworkCore (https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)
- Microsoft.EntityFrameworkCore.Sqlite (https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/)
- Microsoft.EntityFrameworkCore.Design (https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/)

## Contributors
- Isobel Pritchard: UI and ViewModel Development
- Alex Cooke: Persistence and User Authentication/Access Control Development
- Hannah Tchibinda: Business Logic and Domain Model Development
- All: Testing and Documentation
