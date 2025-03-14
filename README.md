# CreditCardRegistration Project

## Overview
This project is a credit card registration system built using Entity Framework Core (EF Core) with a LocalDB backend. It includes three main tables: `CardTypes`, `Users`, and `CreditCards`, along with an initial migration (`20250307164424_InitialCreate.cs`) to set up the database schema and seed data.

## Prerequisites
### Tools:
- .NET Core SDK (version 6.0 or higher).
- Visual Studio (2019 or 2022) with SQL Server Express LocalDB installed (included with Visual Studio by default).

### NuGet Packages:
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design`

## Setup and Running Instructions

### 1. Prepare the Database File
The project uses a LocalDB file named `CreditCardRegistrationDB.mdf` as the database.

#### Locate the File:
- The `.mdf` file is included in the project (typically found in the `bin\Debug` folder after building).
- Copy `CreditCardRegistrationDB.mdf` (and its associated `.ldf` log file, if present) to a directory on your machine (e.g., `C:\Temp`).

#### Attach the File:
- Open Visual Studio.
- Go to **Server Explorer** > right-click on "Data Connections" > select "Add Connection".
- Choose "Microsoft SQL Server Database File" as the data source.
- Browse to the location of `CreditCardRegistrationDB.mdf` and click "OK" to attach it to LocalDB.

### 2. Configure the Connection String
- Open the `appsettings.json` file in the project root.
- Ensure the connection string matches the location of the `.mdf` file. The default configuration is:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CreditCardRegistrationDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "*"
  }
