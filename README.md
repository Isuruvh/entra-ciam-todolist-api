# ToDoListAPI

ASP.NET Core Web API secured with Microsoft Entra External ID (CIAM). The API supports delegated user access and daemon application access using Microsoft Identity Web.

## Features

- JWT bearer authentication with Microsoft Entra External ID
- Scope-based delegated authorization
- App-role authorization for daemon callers
- In-memory Entity Framework Core storage
- GET and POST to-do endpoints
- Separate console client demonstrating client-credentials flow

## Requirements

- .NET SDK 10
- An Entra External ID tenant
- An API app registration exposing these app roles:
  - `ToDoList.Read.All`
  - `ToDoList.ReadWrite.All`
- A daemon/client app registration with the required application permissions and admin consent

## Configuration

Update `appsettings.json` with the values for your tenant and API app registration:

```json
{
  "AzureAd": {
    "Instance": "https://your-tenant-subdomain.ciamlogin.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-api-client-id",
    "Scopes": {
      "Read": "ToDoList.Read ToDoList.ReadWrite",
      "Write": "ToDoList.ReadWrite"
    },
    "AppPermissions": {
      "Read": "ToDoList.Read.All ToDoList.ReadWrite.All",
      "Write": "ToDoList.ReadWrite.All"
    }
  }
}
```

The permission values are space-separated strings because Microsoft Identity Web reads configuration-based permissions as space-separated values.

For the console client, set the daemon application's secret through an environment variable. Never commit the secret:

```powershell
$env:TODO_CLIENT_SECRET = "your-client-secret"
```

The sample client currently uses the client ID and tenant values in `MyTestApp/Program.cs`. Replace those values with your own app registration values before publishing or sharing the project.

## Run the API

From the repository root:

```powershell
dotnet restore
dotnet run
```

The API listens at:

```text
https://localhost:44351
```

The to-do endpoint is:

```text
GET https://localhost:44351/api/todolist
```

The local HTTPS developer certificate may need to be trusted:

```powershell
dotnet dev-certs https --trust
```

## Run the daemon client

Start the API first, then run the client from a second terminal:

```powershell
$env:TODO_CLIENT_SECRET = "your-client-secret"
dotnet run --project MyTestApp/MyTestApp.csproj
```

A successful request prints:

```text
Access token acquired.
Your response is: OK
```

A `401 Unauthorized` response means the token is missing or invalid. A `403 Forbidden` response usually means the token does not contain one of the required app roles or the API configuration does not match the role names.

## API operations

| Method | Route | Permission |
| --- | --- | --- |
| GET | `/api/todolist` | `ToDoList.Read` or `ToDoList.Read.All` |
| POST | `/api/todolist` | `ToDoList.ReadWrite` or `ToDoList.ReadWrite.All` |

## Security notes

- Do not commit client secrets, access tokens, certificates, or local configuration files.
- Rotate any credential that has previously been committed or printed to a terminal.
- The sample uses an in-memory database; use a persistent database and restricted CORS settings in production.
