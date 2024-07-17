# Employee Management System Backend

This is the backend of the Employee Management System, developed using .NET. The backend provides API endpoints for managing employees and handles authentication using JWT.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (Installed via Homebrew on macOS)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/aayebare/employee-backend.git
   cd employee-backend

## Configuration
    in the home directory, use the command `dotnet restore` to restore the .NET depedencies

## Database setup
   - In the appsettings.json file, configure the database as such, 
    `
    {
  "ConnectionStrings": {
    "DefaultConnection": "Host=yourhost;Port=yourport;Database=DBname;Username=yourusername;"
  },
  "Jwt": {
    "Key": "mykey",
    "Issuer": "ayebare",
    "Audience": "myAPIUsers"
  }
}
    `
- Once this is done, run the command `dotnet ef database update` to create new DB schema and then `dotnet ef migrations add <MigrationName>`
    to add a new migration

## Running the Application
- Run the command `dotnet run` to start the application

## API Endpoints

    POST /api/auth/login: Login and get JWT token
    POST /api/auth/signup: Sign up a new user
    Employees

    GET /api/employees: Get all employees
    GET /api/employees/{id}: Get employee by ID
    POST /api/employees: Add a new employee
    PUT /api/employees/{id}: Update an employee
    DELETE /api/employees/{id}: Delete an employee
