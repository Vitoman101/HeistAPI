# Heist API

The Heist API is a RESTful service designed to manage heists and members involved in planning and executing them. It provides endpoints to create, update, and retrieve information about heists and members, as well as manage skills required for each heist.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)

## Features

- **Heist Management:**
  - Create, update, and delete heists.
  - Assign and confirm members for each heist.
  - Track heist status and outcomes.
- **Member Management:**
  - Add new members and update member information.
  - Manage member skills and assign them to heists.
- **Skills and Requirements:**
  - Define required skills for each heist.
  - Validate skill levels and prevent duplicate entries.
- **Outcome Calculation:**
  - Calculate and update heist outcomes based on member confirmations.

## Technologies Used

- **Backend:**
  - ASP.NET Core
  - Entity Framework Core
  - C# language features
- **Database:**
  - SQL Server (or your preferred database system)
- **Tools and Libraries:**
  - Swagger/OpenAPI for API documentation
  - AutoMapper for object-to-object mapping

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version compatible with your project)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or another supported database)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Vitoman101/HeistAPI
   cd HeistAPI

2. Update database connection string in appsettings.json:
   ```bash
   "ConnectionStrings": {
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HeistDb;Trusted_Connection=True;"
   }

### Running the Application
1. You can run application with CMD or Terminal
   ```bash
   dotnet run

or you can just use Visual Studio and https profile to get swagger running

![image](https://github.com/Vitoman101/HeistAPI/assets/17600421/531f8670-387a-4824-989c-7739961c1320)
