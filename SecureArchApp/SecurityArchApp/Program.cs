using Blazored.LocalStorage;
using MudBlazor.Services;
using SecureArchApp.Client.Pages;
using SecureArchApp.Components;
using SecureArchApp.Services;



var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
//TODO das changen!!
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri("https://deine-api-url.de/") };
    return httpClient;
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();
app.UseCors("AllowBlazorClient"); // CORS-Middleware einsetzen

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SecureArchApp.Client._Imports).Assembly);

app.Run();
