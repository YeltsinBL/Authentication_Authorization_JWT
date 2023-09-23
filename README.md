# Authentication_Authorization_JWT

Realización de API Rest para la autenticación y autorización del usuario, utilizando JWT. También de un Login para realizar las pruebas del Api.

## API REST

### NuGet

- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt

### Conexión a la BD

- Se creó una carpeta [`Models`][logintoken] para que se cree automáticamente los modelos de las tablas.
- Mediante el uso de la consola de comando de Nuget se agregó el siguiente comando:

```sh
Scaffold-DbContext "Server=[server_name]; DataBase=[nombre_bd]; Trusted_Connection=True; TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir [nombre de la carpeta creado]
```

- En el archivo [`appsettings.json`][appsettings] se agregó el 'ConnectionStrings' para agregar la conexión a la BD.
- En el archivo [`Program.cs`][programcs] se hizo la referencia a archivo del Context que se autogeneró mediante la consola de nuget para posteriormente utilizar el 'ConnectionStrings'.

### Configuración para el servicio de JWT

- En el archivo [`appsettings.json`][appsettings] se agregó el 'JwtSetting' para agregar la clave secreta que creará el JWT.
- En el archivo [`Program.cs`][programcs] se registró la interfaz y clase del Service
  - Se configuró JWT en base del uso de las credenciales.
  - Se agregó el CORS para permitir hacer peticiones desde otras direcciones.
- [`Models->Custom`][models]:
  - AuthorizationRequest: para las credenciales del Login.
  - AuthorizationResponse: para la respuesta al Login.
  - RefreshTokenRequest: para generar el access y refresh token.
- [`Service`][service]:

  - [IAuthorizationService][iauthorizationservice]: interfaz para las autorizaciones del Token.
  - [AuthorizationService][authorizationservice]: clase heredada donde se implementa los métodos de la interfaz IAuthorizationService y se agrega métodos privados para la creación y guardado de los Access y Refresh Token en la BD.
- [`Controllers`][controller]:

  - [UserController][usercontroller]: api para iniciar sesión y obtener el refresh token.
  - [CountriesController][countriescontroller]: listado solo si ha iniciado sesión.

[//]: # (Enlaces a la documentación)

[appsettings]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/blob/master/LoginToken/appsettings.json>
[logintoken]:  <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken>
[programcs]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/blob/master/LoginToken/Program.cs>
[models]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Models>
[service]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Service>
[iauthorizationservice]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Service/IAuthorizationService.cs>
[authorizationservice]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Service/AuthorizationService.cs>
[controller]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Controllers>
[usercontroller]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Controllers/UserController.cs>
[countriescontroller]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginToken/Controllers/CountriesController.cs>
