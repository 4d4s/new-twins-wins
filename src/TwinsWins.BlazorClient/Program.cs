using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TwinsWins.BlazorClient;
using TwinsWins.BlazorClient.Services;
using MudBlazor.Services;
using Fluxor;
using Blazored.LocalStorage;
using Fluxor.Blazor.Web.ReduxDevTools;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5000") 
});

// API Services
builder.Services.AddScoped<IGameApiService, GameApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();

// SignalR
builder.Services.AddScoped<IGameHubService, GameHubService>();

// MudBlazor
builder.Services.AddMudServices();

// Fluxor State Management
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    
#if DEBUG
    options.UseReduxDevTools();
#endif
});

// Local Storage
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
