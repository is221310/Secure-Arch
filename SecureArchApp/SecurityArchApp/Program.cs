using Blazored.LocalStorage;
using MudBlazor.Services;
using SecureArchApp.Client.Pages;
using SecureArchApp.Components;



var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
try
{
    DotNetEnv.Env.Load();
}
catch (FileNotFoundException)
{
    //in container environment, .env file is not needed 
} 


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
                 ?? "https://localhost:7254/";

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

var app = builder.Build();
app.UseCors("AllowBlazorClient"); // CORS-Middleware einsetzen

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SecureArchApp.Client._Imports).Assembly);

app.Run();
