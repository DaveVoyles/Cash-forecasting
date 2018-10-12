## About
This web frontend allows users to upload a .CSV to blob storage which is then piped to a trained forecasting model and returns a forecast result.

## Repo Structure
1. ui folder = Angular6 front-end (http://localhost:4200)
2. CashForecasting.Api = AspNetCore 2.1 API (http://localhost:5000)

## Requirements
* .NET Core 2.1 - (Installation instructions)[https://blogs.msdn.microsoft.com/benjaminperkins/2018/06/04/how-to-install-asp-net-core-2-1-for-development/]
* Visual Studio 2017  15.7.3
* [Angular CLI](https://cli.angular.io/)

## Setup
1. Create Azure Active Directory app
2. Add reply URLs to Azure AD App:
	- http://localhost:4200/home
	- http://localhost:4200
3. Add `Sign in and read user profile` delegated permissions of `Microsoft Graph` API
4. Edit Azure AD App manifest `"oauth2AllowImplicitFlow": true,`
5. Update the `environment.ts` and `environment.prod.ts` with Application Id of newly registered Azure AD app.

## Running
To run the API, you can either start use Visual Studio to run/debug API project or use appropriate `dotnet` commands.

To run the Angular6 app, from the root `ui` folder run: `npm install` and then `ng serve` 

## To Disable AAD
Azure has been acting up while working on this project, so you can navigate to `app.module.ts` and comment the line `canActivate: [AdalGuard]` to prevent authentication. 

Afterwards, you can directly navigate to `http://localhost:4200/files/upload` to upload files to blob storage.

## Items to change for production
- Connection String in `AzureBlobManager.cs` should stored outside in an environment file outside of the project
- Container name in `FileController.cs`. EX:	*abm.ContainerName = 'myContainer'*

## Images


