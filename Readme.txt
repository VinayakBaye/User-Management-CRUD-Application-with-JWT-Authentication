USER MANAGEMENT SYSTEM
======================

Technology Stack
----------------
Backend:
- .NET 8 Web API
- ASP.NET Core
- Entity Framework Core 8
- SQLite
- JWT Authentication

Frontend:
- React
- Vite
- Axios

Architecture
------------
UserManagement/
|
+-- src/
|   |
|   +-- UserManagement.Api/
|   |   +-- Controllers/
|   |   |   +-- AuthController.cs
|   |   |   +-- UsersController.cs
|   |   |
|   |   +-- Program.cs
|   |   +-- appsettings.json
|   |
|   +-- UserManagement.Domain/
|   |   +-- Entities/
|   |       +-- User.cs
|   |
|   +-- UserManagement.Infrastructure/
|       +-- Persistence/
|           +-- AppDbContext.cs
|           +-- Migrations/
|
+-- React Frontend/
    +-- src/
        +-- services/
            +-- api.js
            +-- authService.js
            +-- userService.js


REQUIREMENTS
------------
- .NET 8 SDK
- Node.js
- npm
- SQLite
- Visual Studio / VS Code
- Optional: DB Browser for SQLite


BACKEND SETUP
-------------

1. Navigate to the API project:

   cd src\UserManagement.Api


2. Restore NuGet packages:

   dotnet restore


3. Check .NET version:

   dotnet --version

   The project targets .NET 8.


DATABASE CONFIGURATION
----------------------

The application uses SQLite.

In:

   UserManagement.Api\appsettings.json

configure:

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=usermanagement.db"
  }
}


ENTITY MODEL
------------

User contains:

- Id       : Primary Key
- Name     : Required, 2-100 characters
- Age      : Required, 0-120
- City     : Required
- State    : Required
- Pincode  : Required, 4-10 characters
- CreatedAtUtc : Date/time


ENTITY FRAMEWORK CORE
---------------------

EF Core version must match the .NET version.

For this project:

.NET              : 8
EF Core           : 8.x
SQLite provider   : 8.x
EF Design         : 8.x
dotnet-ef         : 8.x


Install EF Core packages if required:

dotnet add package Microsoft.EntityFrameworkCore --version 8.0.20

dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.20

dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.20


MIGRATIONS
----------

The DbContext is located in:

UserManagement.Infrastructure

The startup project is:

UserManagement.Api


Create a migration:

dotnet ef migrations add InitialCreate --project src\UserManagement.Infrastructure --startup-project src\UserManagement.Api


Update the database:

dotnet ef database update --project src\UserManagement.Infrastructure --startup-project src\UserManagement.Api


If the database needs to be recreated during development:

1. Delete:

   usermanagement.db

2. Run:

   dotnet ef database update --project src\UserManagement.Infrastructure --startup-project src\UserManagement.Api


DATABASE
--------

SQLite database file:

   usermanagement.db

Main tables:

   Users
   __EFMigrationsHistory


To inspect the database, use DB Browser for SQLite:

https://sqlitebrowser.org/


Example SQL:

SELECT * FROM Users;


BACKEND API
-----------

Default backend URL:

https://localhost:61076


Authentication
--------------

Login endpoint:

POST /api/auth/login


Example request:

{
  "ClientID": "xxxxxxxxxxxxxxxxx",
  "ClientSecret": "xxxxxxxxxxxxxxxxx"
}


Example response:

{
  "token": "JWT_TOKEN"
}


The React application stores the JWT token in:

sessionStorage

Key:

access_token


The Axios interceptor automatically adds the token to protected requests:

Authorization: Bearer <token>


USER API
--------

Create User:

POST /api/users


Example request:

{
  "name": "Vinayak Baye",
  "age": 30,
  "city": "Navi Mumbai",
  "state": "Maharashtra",
  "pincode": "400614"
}


Get Users:

GET /api/users


Get User:

GET /api/users/{id}


Update User:

PUT /api/users/{id}


Delete User:

DELETE /api/users/{id}


REACT FRONTEND SETUP
--------------------

Navigate to the React application:

cd <react-project-folder>


Install dependencies:

npm install


Create a .env file in the React project root:

VITE_BACKEND_API_BASE_URL=https://localhost:61076/api

VITE_AUTH_CLIENT_ID=xxxxxxxxxxxxxxxxxxxxxxxxx

VITE_AUTH_CLIENT_SECRET=xxxxxxxxxxxxxxxxxxxxxxxxx


IMPORTANT:
After changing .env, restart Vite.


Start React:

npm run dev


The Vite application normally runs at:

http://localhost:5173


AXIOS CONFIGURATION
-------------------

The Axios instance is located at:

src/services/api.js


Example:

import axios from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_BACKEND_API_BASE_URL,
    headers: {
        "Content-Type": "application/json"
    }
});


The request interceptor reads:

sessionStorage.getItem("access_token")


and adds:

Authorization: Bearer <token>


AUTHENTICATION FLOW
-------------------

React Application
       |
       v
authService.login()
       |
       v
POST /api/auth/login
       |
       v
AuthController
       |
       v
JWT Token Generated
       |
       v
sessionStorage
       |
       v
access_token
       |
       v
Axios Interceptor
       |
       v
Authorization: Bearer <token>
       |
       v
POST /api/users
       |
       v
UsersController
       |
       v
Entity Framework Core
       |
       v
SQLite


TROUBLESHOOTING
---------------

1. React calls localhost:5173 instead of the API

Make sure Axios uses:

import api from "./api";

and:

api.post("/auth/login", ...)

NOT:

axios.post("/auth/login", ...)


2. API returns 404 for /auth/login

Check AuthController:

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(...)
    {
        ...
    }
}


3. JWT returns 401

Check that:

- Login successfully generates a token.
- Token is stored in sessionStorage.
- The Axios interceptor reads access_token.
- Authorization header contains:

  Bearer <token>

- JWT authentication middleware is configured correctly.


4. EF Core migration error

Make sure:

- API targets net8.0.
- EF Core packages are 8.x.
- Microsoft.EntityFrameworkCore.Design is installed in the API startup project.
- dotnet-ef is version 8.x.


5. SQLite database cannot be found

Check:

appsettings.json

ConnectionStrings -> DefaultConnection


6. Check database records

Open:

usermanagement.db

using DB Browser for SQLite.

Run:

SELECT * FROM Users;


DEVELOPMENT COMMANDS
--------------------

Restore:

dotnet restore


Build:

dotnet build


Run API:

dotnet run --project src\UserManagement.Api


Create migration:

dotnet ef migrations add InitialCreate --project src\UserManagement.Infrastructure --startup-project src\UserManagement.Api


Update database:

dotnet ef database update --project src\UserManagement.Infrastructure --startup-project src\UserManagement.Api


Run React:

npm run dev


PROJECT STATUS
--------------

Backend:
- ASP.NET Core Web API
- JWT Authentication
- Entity Framework Core
- SQLite
- User CRUD APIs

Frontend:
- React
- Vite
- Axios
- Axios request interceptor
- JWT token storage


SECURITY NOTE
-------------

The client ID and client secret stored in a Vite .env file are exposed to the browser after the application is built.

Do NOT store real production secrets in VITE_* environment variables.

For production, authentication should be implemented using a proper server-side authentication flow and secrets should remain on the server.