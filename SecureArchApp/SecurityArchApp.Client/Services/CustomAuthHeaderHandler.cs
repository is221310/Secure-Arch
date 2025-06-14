using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SecureArchApp.Client.Services
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;

        public AuthorizationMessageHandler(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Token aus localStorage holen
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "access_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Optional: Token entfernen
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "access_token");

                // Redirect zu Login-Seite
                _navigationManager.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}");
            }

            return response;
        }
    }


    }

