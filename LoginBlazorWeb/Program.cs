using Blazored.LocalStorage;
using LoginBlazorWeb;
using LoginBlazorWeb.Extensiones;
using LoginBlazorWeb.Service;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Conexión a la URL del API
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5283/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddHttpClient("registerApi", option =>
//{
//	//option.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrl:url"));
//    option.BaseAddress = new Uri("http://localhost:5283/");
//}).AddHttpMessageHandler<CustomHttpHandler>(); // se ejecuta cada vez que se hace una petición

# region Configuración para utilizar appsetting.Json
var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }; // indicamos la URL Base
builder.Services.AddScoped(sp => http); // agregamos el http al Scoped
using var response = await http.GetAsync("appsettings.json"); // indicamos el archivo json para leer
using var stream = await response.Content.ReadAsStreamAsync(); // leemos el arhivo json
builder.Configuration.AddJsonStream(stream); // agregamos el contenido del json a la configuración

#endregion

#region Agregar identificador al HttClient y la URL de la API usando appsetting.Json
var valorregistro = builder.Configuration.GetValue<string>("httpClient:registrarHttp");
var valoruri = builder.Configuration.GetValue<string>("httpClient:urlApi");
builder.Services.AddHttpClient(valorregistro!, option =>
{
    option.BaseAddress = new Uri(valoruri!);
}).AddHttpMessageHandler<CustomHttpHandler>(); // se ejecuta cada vez que se hace una petición

#endregion

#region Registrar los archivos creados
// Archivos que interactúan con las vistas
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationExtension>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomHttpHandler>();

#endregion

await builder.Build().RunAsync();
