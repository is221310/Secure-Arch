using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;

using SecureArchApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddTransient<AuthorizationMessageHandler>();
builder.Services.AddHttpClient("AuthorizedClient")
    .AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));
builder.Services.AddHttpClient("SecureAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7254/");
});
builder.Services.AddScoped<ApiHelper>();


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
