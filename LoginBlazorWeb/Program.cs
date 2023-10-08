using Blazored.LocalStorage;
//using Blazored.SessionStorage;
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
builder.Services.AddHttpClient("registerApi", option =>
{
	//option.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrl:url"));
    option.BaseAddress = new Uri("http://localhost:5283/");
}).AddHttpMessageHandler<CustomHttpHandler>(); // se ejecuta cada vez que se hace una petición

#region Registrar los archivos creados
// Archivos que interactúan con las vistas
builder.Services.AddBlazoredLocalStorage();
//builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationExtension>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomHttpHandler>();

#endregion

await builder.Build().RunAsync();
