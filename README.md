# Authentication_Authorization_JWT

Realización de API Rest para la autenticación y autorización del usuario, utilizando JWT.

## NuGet

- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt

## Conexión a la BD

- Se creó una carpeta `[Models][models]` para que se cree automáticamente los modelos de las tablas.
- Mediante el uso de la consola de comando de Nuget se agregó el siguiente comando:

```sh
Scaffold-DbContext "Server=[server_name]; DataBase=[nombre_bd]; Trusted_Connection=True; TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir [nombre de la carpeta creado]
```

- En el archivo `[appsettings.json][appsettings]` se agregó el 'ConnectionStrings' para agregar la conexión a la BD.
- En el archivo `[Program.cs][programcs]` se hizo la referencia a archivo del Context que se autogeneró mediante la consola de nuget para posteriormente utilizar el 'ConnectionStrings'.

[//]: # (Enlaces a la documentación)

[appsettings]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/blob/master/LoginToken/appsettings.json>
[models]:  <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken>
[programcs]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/blob/master/LoginToken/Program.cs>
