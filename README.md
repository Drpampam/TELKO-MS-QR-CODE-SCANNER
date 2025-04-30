

# TELKO-MS QR Code Scanner Microservice

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue) ![License: MIT](https://img.shields.io/badge/License-MIT-green)

## Overview

This project implements a **QR Code Scanner** microservice for managing employee contact information and generating contact QR codes. It is built using ASP.NET Core (C#) on the .NET 8 platform. Contacts (full names, phones, emails, titles, etc.) are stored in a SQL Server database via Entity Framework Core, and QR codes are generated from contact data in the vCard format. The service provides a RESTful HTTP API with Swagger documentation for easy integration and testing. The code is organized in a clean layered architecture (separating API, business logic, and data access), and includes built-in health checks, structured logging, and CORS support.

## Features

- **Health Check Endpoint:** A simple `GET /` endpoint returns a success message (including a response code) to verify the service is running.
- **Contact Listing:** `GET /api/v1/contacts/all-contacts` returns all employee contacts. Supports optional filtering by phone or date range, and includes pagination (page number and size) in the response.
- **QR Code Generation:** `POST /api/v1/qrcode/{phone}/qrcode` generates a PNG QR code for the contact with the given phone number. The QR code encodes a **vCard** containing the contact's full name, company, title, phone, email, and LinkedIn URL.
- **Swagger API Documentation:** Integrated Swagger UI (via Swashbuckle) provides interactive API documentation and testing.
- **Logging:** Serilog is configured for structured logging of requests and errors.
- **CORS:** Configured to allow requests from any origin, enabling cross-domain use.
- **EF Core Migrations:** Database migrations are included to create/update the schema (table `EmployeeContacts`).

## Technologies Used

- **ASP.NET Core 8.0 (C#):** A free, open-source, cross-platform framework for building modern web APIs ([ASP.NET Core, an open-source web development framework | .NET](https://dotnet.microsoft.com/en-us/apps/aspnet#:~:text=ASP)).
- **Entity Framework Core:** A lightweight, extensible ORM for .NET, enabling data access via C# objects ([Overview of Entity Framework Core - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/#:~:text=Entity%20Framework%20,Entity%20Framework%20data%20access%20technology)).
- **SQL Server:** The relational database (default) for persisting contact data.
- **QRCoder:** A C# library for generating QR code images ([GitHub - codebude/QRCoder: A pure C# Open Source QR Code implementation](https://github.com/codebude/QRCoder#:~:text=QRCoder%20is%20a%20simple%20library%2C,frameworks%20can%20be%20found%20here)). Used to convert vCard strings into PNG byte arrays.
- **Swashbuckle (Swagger):** Generates interactive API documentation (Swagger UI) from the code ([
        NuGet Gallery
        | Swashbuckle.AspNetCore.Swagger 8.1.1
    ](https://www.nuget.org/packages/swashbuckle.aspnetcore.swagger/#:~:text=Swagger%20tooling%20for%20APIs%20built,NET%20Core)).
- **Serilog:** A diagnostic logging library for .NET applications ([Serilog — simple .NET logging with fully-structured events](https://serilog.net/#:~:text=Like%20many%20other%20libraries%20for,NET%20platforms)).
- **Pagination Helper:** Custom extension methods to support paging large contact lists.
- **Repository & Unit of Work:** Data access patterns used for abstraction and transaction management.

## Installation

1. **Prerequisites:** Install the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (which includes ASP.NET Core) and ensure you have a SQL Server instance available.
2. **Clone the Repository:**  
   ```bash
   git clone <repository-url>
   cd TELKO-MS-QR-CODE-SCANNER-master
   ```
3. **Configure Database:** In `src/Api/appsettings.json`, set the `ConnectionStrings:DataConnectionStrings` (or use `DefaultConnection`) to your SQL Server connection string.  
4. **Restore & Build:**  
   ```bash
   dotnet restore TELKO-MS-APP.sln
   dotnet build TELKO-MS-APP.sln
   ```  
   (Alternatively, open `TELKO-MS-APP.sln` in Visual Studio and build.)
5. **Apply Migrations:** Run the EF Core migrations to create/update the database schema:  
   ```bash
   cd src/Api
   dotnet tool install --global dotnet-ef   # if needed
   dotnet ef database update
   ```  
   This creates the `EmployeeContacts` table as defined in the code.

## Running the Application

- **Local Run:** From the `src/Api` folder, run:  
  ```bash
  dotnet run
  ```  
  The service will start (by default listening on `https://localhost:5001` and `http://localhost:5000`). Visit `https://localhost:5001/swagger` to view the Swagger UI.
- **Via Docker:** A `Dockerfile` is included for containerization. Build and run with:  
  ```bash
  docker build -t telko-qr-scanner .
  docker run -d -p 5001:80 --name telko-qr telko-qr-scanner
  ```  
  The API will then be accessible at `http://localhost:5001` (HTTP) or `https://localhost:5001` (HTTPS).

## API Endpoints

- `GET /`  
  *Health check.* Returns HTTP 200 with a simple success message and response code.
- `GET /api/v1/contacts/all-contacts`  
  *List Contacts.* Retrieves employee contacts in a paginated response. Supports query parameters:
  - `pageNumber` (default 1)  
  - `pageSize` (default 20)  
  - `Phone` (exact-match filter by phone number)  
  - `StartDate`, `EndDate` (filter by creation date range, format `YYYY-MM-DD`)  

  **Response:** JSON with fields like `ResponseCode`, `Message`, and `Data`. `Data` contains `Items` (list of `EmployeeContactReponseDto` objects with `FullName` and `Phone`), along with paging metadata (`PageNumber`, `PageSize`, `TotalCount`, etc.).
- `POST /api/v1/qrcode/{phone}/qrcode`  
  *Generate QR Code.* Produces a QR code PNG image for the contact with the specified `{phone}`.  

  - **Success:** HTTP 200 with `Content-Type: image/png` and the QR code image data.  
  - **Errors:** 400 if contact not found or invalid input, 500 on server error.
- **Swagger UI:** `GET /swagger` opens the interactive API documentation (when enabled).

## Configuration

- **Connection Strings:** The `appsettings.json` key `ConnectionStrings:DataConnectionStrings` sets the SQL Server connection string (override with `ConnectionStrings__DataConnectionStrings` environment variable if needed).
- **Logging:** Controlled via the `Logging` section in `appsettings.json`. Adjust log levels (`Information`, `Warning`, etc.) as desired.
- **CORS Policy:** By default allows any origin. Modify the policy in `Program.cs` if you need to restrict it.
- **URLs:** Default URLs are defined in `Properties/launchSettings.json` (HTTPS port 5001 by default). Use the `ASPNETCORE_URLS` environment variable to customize.

## Contributing

Contributions are welcome! Please fork the repository, create a branch for your changes, and submit a pull request.  

- Follow existing C# coding conventions and maintain consistency.  
- Write unit tests for any new functionality.  
- Provide clear descriptions of changes in your PR.

## License

This project is licensed under the **MIT License**. See [LICENSE.txt](LICENSE.txt) for details.

## Contact and Acknowledgements

- **Maintainer:** (Name/Team) – For questions or support, please open an issue on GitHub.  
- **Acknowledgements:** This service uses several open-source libraries:
  - **ASP.NET Core** (web API framework) ([ASP.NET Core, an open-source web development framework | .NET](https://dotnet.microsoft.com/en-us/apps/aspnet#:~:text=ASP))  
  - **Entity Framework Core** (ORM/data access) ([Overview of Entity Framework Core - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/#:~:text=Entity%20Framework%20,Entity%20Framework%20data%20access%20technology))  
  - **QRCoder** (QR code generation) ([GitHub - codebude/QRCoder: A pure C# Open Source QR Code implementation](https://github.com/codebude/QRCoder#:~:text=QRCoder%20is%20a%20simple%20library%2C,frameworks%20can%20be%20found%20here))  
  - **Swashbuckle (Swagger)** (API docs) ([
        NuGet Gallery
        | Swashbuckle.AspNetCore.Swagger 8.1.1
    ](https://www.nuget.org/packages/swashbuckle.aspnetcore.swagger/#:~:text=Swagger%20tooling%20for%20APIs%20built,NET%20Core))  
  - **Serilog** (logging) ([Serilog — simple .NET logging with fully-structured events](https://serilog.net/#:~:text=Like%20many%20other%20libraries%20for,NET%20platforms))  

  Thank you to the authors and contributors of these projects. 

