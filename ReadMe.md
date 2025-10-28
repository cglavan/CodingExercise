# My understanding
I was tasked to create a simple API which allows a user to perform the following:

1. Retrieve a list of investments for a user
2. Retrieve the details of a specific investment for a user

# Assumptions
- Each user  has a unique identifier.
- Each user can have multiple investments.
- Each investment has a unique identifier.
- The API user has been authenticated and authorized to access a list of user investments.
- The API user has been authenticated and authorized to access the details of a specific investment.
- For testing purposes the database should be seeded with sample data.
- The API should handle errors gracefully and return appropriate HTTP status codes.
- Logging should be implemented to track API usage and errors.
- The API should be documented using OpenAPI/Swagger for easy consumption by clients.
- The API should follow RESTful principles for resource management.
- Basic tests using a testing framework like xUnit or NUnit should be included to verify the functionality of the API endpoints.

# Implementation
The API is implemented using ASP.NET Core Web API framework. It includes the following components:
- The project targets .NET 8.0 and uses C# 12.0 features.
- The project uses Entity Framework Core with a SQL Server database to store user and investment data.

Folder Structure:
- Models: Define the data structures for an Investment.
- Controllers: Handle HTTP requests and responses for the API endpoints.
- Services: Contains the logic for validating api keys.
- Data: Contains the database context for Entity Framework Core and associated DTOs.
- Common: Contains common utilities such as encryption helper classes.
- Migrations: Contains the Entity Framework Core migrations for database schema management.

Additionally, the project includes the following features:
- Serilog: Handles error logging.
- Swagger: Provides API documentation.
- Unit Tests: Verify the functionality of the API endpoints.
- Seed Data: Initializes the database with sample data for testing purposes.
- API Key Authentication: Secures the API endpoints.
- Encryption: Used for encryption connection strings.
- Configuration (AppSettings): Manages application settings and configurations.
- Dependency Injection: Manages service lifetimes and dependencies.

# Usage

The application can be run locally using the .NET CLI or Visual Studio. Once running, the API endpoints can be accessed via HTTP requests sent to https://localhost:7246. The Swagger UI can be used to explore and test the API endpoints.  The 2 main endpoints for this exercise can be found at the following URLs:

	https://localhost:7246/api/Investments/user?userId={userId}

	https://localhost:7246/api/Investments/user/{investmentId}

The UserId for the single test user for this exercise is: 11111111-1111-1111-1111-111111111111

The API key for accessing the endpoints is through Swagger, Postman or any other development tool is as follows:

	header: x-api-key
	value: NuiX123!

The appSettings.json file contains an "AppConfig" entry for testing against different environments (i.e. "dev", "stage" and "prod").  You can change the value of the "AppConfig" entry to test against different environments.  Each environment has its own appsettings.{environment}.json file which contains the appropriate settings for that environment.

The appSettings.json file also contains an encrypted connection string for the database. The encryption key is stored in the "Encryption settings" section, along with the salt and vector values.  These values should be stored in a secure location outside of the application (i.e. Azure Key Vault, AWS Secrets Manager, etc.) for production use.  However, for the purposes of this exercise I have included them in the appSettings.json file.

The database(s) are stored in my personal Azure subscription.  I will provide public access to the database(s) throughout the interview process.  Once this process concludes I will remove the database, access and any associated resources.

Logging level is configured using the "LoggingLevel" entry of the appSettings.json file.  The current configuration is set to log "debug" level messages in a development environment and "warning" level messages in a production environment.  Environments are configured using the appropriate appsettings.json file for the desired environment.

# xUnit test

Basic tests are included in the InvestmentPerformaceApiTest.csproj project.  These tests can be run using the .NET CLI or Visual Studio Test Explorer.  The tests cover the main functionality of the API endpoints.  The specific commands that I used during testing are as follows:

	dotnet restore
	dotnet build
	dotnet test

The results of running the tests are as follows:

	Test summary: total: 2, failed: 0, succeeded: 2, skipped: 0, duration: 27.8s