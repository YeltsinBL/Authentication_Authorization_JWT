using Blazored.SessionStorage;
using LoginBlazorWeb;
using LoginBlazorWeb.Extensiones;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Conexión a la URL del API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5283/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registrar los archivos creados
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationExtension>();
builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
