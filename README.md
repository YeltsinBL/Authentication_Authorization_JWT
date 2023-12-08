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
  - Se registró la interfaz y clase de la AuthorizationService en el Scoped.
  - Se agregó el CORS para permitir hacer peticiones desde otras direcciones.
- [`Models->Custom`][models]:
  - AuthorizationRequest: para las credenciales del Login.
  - AuthorizationResponse: para la respuesta al Login.
  - RefreshTokenRequest: para generar el access y refresh token.
  - RegisterRequest: para las credenciales al crear una cuenta
- [`Service`][service]:
  - [IAuthorizationService][iauthorizationservice]: interfaz para las autorizaciones: login con Token y registrar cuenta.
  - [AuthorizationService][authorizationservice]: clase heredada donde se implementa los métodos de la interfaz IAuthorizationService y se agrega métodos privados para la creación y guardado del Access y Refresh Token en la BD.
- [`Controllers`][controller]:
  - [UserController][usercontroller]: api para iniciar sesión, obtener el refresh token y crear cuenta.
  - [CountriesController][countriescontroller]: listado solo si ha iniciado sesión.

## Web

### NuGet Web

- Blazored.LocalStorage
- Blazored.SessionStorage
- Microsoft.AspNetCore.Components.Authorization
- Microsoft.Extensions.Http
- Newtonsoft.Json

### Creación y Configuración del Login

- [wwwroot][wwwrootView]
  - Creación del archivo `appsetting.json` para configurar la conexión con el api y sus servicios.
- [Models][modelsView]
  - LoginDTO: credenciales del login.
  - RefreshTokenDTO: credenciales para volver a generar el token.
  - SessionDTO: respuesta del api.
- [Extensiones][extensionesView]
  - StorageExtension: métodos para guardar y obtener los datos en SessionStorage y LocalStorage.
  - AuthenticationExtension: lógica con los datos del usuario autenticado para guardarlo y obtenerlo de la SessionStorage o LocalStorage.
  - CustomHttpHandler: clase heredada del `DelegatingHandler` y sirve de lógica para verificar si el usuario a inciado sesión, qué apis necesitan authorización y hacer la petición para volver a generar un nuevo token.
  - Utility: creación de métodos generales para: leer el token, generar código SHA256 y obtener el RequestUri desde appsettings.
- [Service][serviceView]
  - IAuthService: interfaz con el método del login.
  - AuthService: clase heredada del IAuthService que implementa la lógica del login.
- [Shared][sharedView]
  - LoginLayout: se creó este layout para el login.
  - MainLayout: se agregó la opción de Cerrar Sesión.
  - NavMenu: se agregó una autorización de roles.
- _Imports: se agregó el LocalStorage.
- App: agregamos la verificaión si se ha iniciado sesión y tiene permiso de roles para visualizar los formularios.
- Program:
  - Se agregó la configuración para el uso del appsettings y lo agregamos a la congifuración del sistema.
  - Agregamos el HttpClient al servicio del sistema, obteniendo los datos desde el appsettings.
  - Registramos los archivos creados que se utilizan por todo el sistema.

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
[wwwrootView]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginBlazorWeb/wwwroot>
[modelsView]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginBlazorWeb/Models>
[extensionesView]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginBlazorWeb/Extensiones>
[serviceView]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginBlazorWeb/Service>
[sharedView]: <https://github.com/YeltsinBL/Authentication_Authorization_JWT/tree/master/LoginBlazorWeb/Shared>
