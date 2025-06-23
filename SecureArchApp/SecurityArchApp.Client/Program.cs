
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using SecureArchApp.Client.Services;
using SecureArchApp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddTransient<AuthorizationMessageHandler>();
builder.Services.AddTransient<CredentialsHandler>();


var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var config = await http.GetFromJsonAsync<Config>("appsettings.json");

builder.Services.AddSingleton(config!);

builder.Services.AddHttpClient("SecureAPI", client =>
{
    client.BaseAddress = new Uri(config?.ApiBaseUrl ?? "https://localhost2:7254");
})
.AddHttpMessageHandler<CredentialsHandler>();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("SecureAPI");
});

await builder.Build().RunAsync();

