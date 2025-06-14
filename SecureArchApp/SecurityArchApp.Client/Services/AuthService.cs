using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;


namespace SecureArchApp.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public class AuthResponse
        {
            public string Username { get; set; }
            public string Access_Token { get; set; }
            public DateTime Access_Token_Expiration { get; set; }
            public string Refresh_Token { get; set; }
            public DateTime Refresh_Token_Expiration { get; set; }
            public string Token_Type { get; set; }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var payload = new { username, password };

            var response = await _http.PostAsJsonAsync("http://localhost:8000/auth/token", payload);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            await _localStorage.SetItemAsync("access_token", result.Access_Token);
            await _localStorage.SetItemAsync("access_token_expiration", result.Access_Token_Expiration);
            return true;
        }

        public async Task<string> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("access_token");
        }
    }
}
