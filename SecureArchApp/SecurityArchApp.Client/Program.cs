using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

using SecureArchApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Register the CredentialsHandler
builder.Services.AddTransient<CredentialsHandler>();

// HttpClient mit CredentialsHandler registrieren
builder.Services.AddHttpClient("SecureAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7254");
})
.AddHttpMessageHandler<CredentialsHandler>();

// Optional: Falls du Default HttpClient verwenden willst (für DI via @inject HttpClient)
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("SecureAPI");
});

await builder.Build().RunAsync();