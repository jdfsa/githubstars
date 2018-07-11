# GitHub Stars
GitHub Stars allows a user star a GitHub repository with less than 2 clicks, by fetch a specific user by his or her name/nickname.
It consists of two basic application tiers, which are:
- API/Backend
- WebApp/Frontend

Those tiers are described in detail (i.e.. as how to run them locally) along this document.

---

## About the solution

### API
The API tier was developed using .Net Core 2 WebApi. It serves as a bridge between the GitHub GraphQL API and the WebApp and provides no more than four endpoints to interact with GitHub.

The authentication flow are as follows:
1. POST '/api/auth/url' returns the GitHub authentication page address;
2. POST '/api/auth/token' must be called with the "code" returned from GitHub in step 1, which returns the access_token to be used throughout the application use.

The endpoints bellow can be used to fetch data from GitHub and all of them require the token generated in the step 2 above:

GET '/api/user' - retrieves data of the logged user (id, login, name and avatar).

GET '/api/repository' - returns the user and his/her repositories given the "search" criteria, which corresponds to the user name or nickname to be retrieved.

POST '/api/repository' - Sets/unsets a star given a repository and the logged user.

### WebApp
The WebApp was developed using Angular 5 and it just responds to the user interaction in the frontend tier. Initially it validates whether the user has an active session by checking the existing of a token in the local storage. If there is no token (it's invalid, given any request which it's required), the user is redirected to the GitHub login page, which lists the permission required so the user can interact with the application.

---

## Prerequisites

Besides cloning the repository content, the host must have all necessary artifacts to run any .NET Core and Angular 5 applications, which includes:
- .Net Core libraries
- Nodejs
- @angular/cli globally installed

References:
- https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites?tabs=netcore2x
- https://angular.io/guide/quickstart

---

## Running locally

### Api

It's easier to run the API using an IDE like Visual Studio Community 2017. But it can also be acomplished by using a console (i.e. powershell):

1. Build the application:
```dotnet build```

2. Run the WebApi
```dotnet run```

3. The console will output the port which the app is runing on.

### WebApp

1. Install the @angular/cli if it's yet not installed
```npm install -g @angular/cli```

2. Change the endpoints in the file /webapp/src/environments/environment.ts accordingly to the address/port the Api is running

3. Install the @angular dependences
```npm install```

4. Run the application
```ng serve```
