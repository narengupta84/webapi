# Dotnet Core API Project with Entity Framework
This is a README file for a .NET Core API project that utilizes Entity Framework for data access. 

## Prerequisites
To run this project, ensure you have the following prerequisites installed:

.NET Core SDK (version 6.0 or later)
Entity Framework Core (version 7.0.5 or later)
A relational database SQL Server. (In this project I used SQL Server from https://www.cloudclusters.io/)

## Steps to Migrate Initial Data to Database
First: Delete the Migrations folder from the project.
Second: Open Package Manager Console run below to query
a. Add-Migration IntialCreate
b. Update-Database

Third: Open cmd at root directory of the project and run this migrate command
a. dotnet run seeddata

To get the token please use this credential
URL: https://localhost:7246/api/Auth
Body:
{
  "id": 1,
  "name": "Pikachu"
}

## Unit Testing
For unit testing have used **FakeItEasy** & **FluentAssertions**

## License
This project is licensed under the MIT License. Feel free to use, modify, and distribute the code as needed.


