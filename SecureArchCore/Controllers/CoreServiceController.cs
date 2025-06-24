using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SecurityArch.Controllers;

[ApiController]
[Route("[controller]")]
public class CoreServiceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;


    public CoreServiceController(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }
   
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> GetKunden()
    {
        var kunden = await _context.Kunden
            .Include(k => k.Users)
            .Include(k => k.Sensoren)
            .ToListAsync();

        return Ok(kunden);
    }

    [HttpGet("getrole")]
    [Authorize]
    public IActionResult GetRole()
    {
        var roleClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "role" ||
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

        if (roleClaim == null)
            return NotFound("Rollenclaim nicht gefunden.");

        return Ok(new { Role = roleClaim.Value });
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var username = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        if (string.IsNullOrEmpty(username))
            return Unauthorized("Kein Benutzername im Token.");

        var user = await _context.Users
            .Include(u => u.Kunde)
            .FirstOrDefaultAsync(u => u.email == username);

        if (user == null)
            return NotFound("Benutzer nicht gefunden.");

        return Ok(new
        {
            Username = user.email,
            Kunde = new
            {
                user.Kunde.kunden_id,
                user.Kunde.kunden_name
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetKunde(int id)
    {
        var kunde = await _context.Kunden
            .Include(k => k.Users)
            .Include(k => k.Sensoren)
            .FirstOrDefaultAsync(k => k.kunden_id == id);

        if (kunde == null)
            return NotFound();

        return Ok(kunde);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            return BadRequest("Username und Passwort müssen gesetzt sein.");

        var baseUrl = Environment.GetEnvironmentVariable("AUTH_API_BASE_URL");
        if (string.IsNullOrEmpty(baseUrl))
            return StatusCode(500, "AUTH_API_BASE_URL nicht gesetzt.");

        var apiUrl = $"{baseUrl.TrimEnd('/')}/auth/token";

        var response = await _httpClient.PostAsJsonAsync(apiUrl, new
        {
            username = loginRequest.Username,
            password = loginRequest.Password
        });

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Authentifizierung fehlgeschlagen.");

        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("access_token", out var accessTokenElement) &&
            root.TryGetProperty("refresh_token", out var refreshTokenElement))
        {
            var accessToken = accessTokenElement.GetString();
            var refreshToken = refreshTokenElement.GetString();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("auth-token", accessToken, cookieOptions);
            Response.Cookies.Append("refresh-token", refreshToken, cookieOptions);

            return Ok();
        }

        return BadRequest("Token konnte nicht extrahiert werden.");
    }

    [HttpPost]
    [Route("refresh")]
    [Authorize]
    public async Task<IActionResult> RefreshToken()
    {
        var baseUrl = Environment.GetEnvironmentVariable("AUTH_API_BASE_URL");
        if (string.IsNullOrEmpty(baseUrl))
            return StatusCode(500, "AUTH_API_BASE_URL nicht gesetzt.");

        var username = User.Claims.FirstOrDefault(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(username))
            return Unauthorized("Benutzername konnte nicht aus dem Token gelesen werden.");

        var refreshToken = Request.Cookies["refresh-token"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Kein Refresh-Token im Cookie gefunden.");

        var apiUrl = $"{baseUrl.TrimEnd('/')}/auth/refresh";

        var response = await _httpClient.PostAsJsonAsync(apiUrl, new
        {
            username = username,
            refresh_token = refreshToken
        });

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Token-Refresh fehlgeschlagen.");

        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("access_token", out var accessTokenElement))
        {
            var accessToken = accessTokenElement.GetString();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("auth-token", accessToken, cookieOptions);

            return Ok();
        }

        return BadRequest("Neue Tokens konnten nicht extrahiert werden.");
    }

    [HttpPost]
    [Route("refreshsensor")]
    public async Task<IActionResult> RefreshSensor()
    {
        var baseUrl = Environment.GetEnvironmentVariable("AUTH_API_BASE_URL");
        if (string.IsNullOrEmpty(baseUrl))
            return StatusCode(500, "AUTH_API_BASE_URL nicht gesetzt.");

        var username = User.Claims.FirstOrDefault(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(username))
            return Unauthorized("Benutzername konnte nicht aus dem Token gelesen werden.");

        var refreshToken = Request.Cookies["refresh-token"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Kein Refresh-Token im Cookie gefunden.");

        var apiUrl = $"{baseUrl.TrimEnd('/')}/auth/token/sensor/refresh";

        var response = await _httpClient.PostAsJsonAsync(apiUrl, new
        {
            username = username,
            refresh_token = refreshToken
        });

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Token-Refresh fehlgeschlagen.");

        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("access_token", out var accessTokenElement))
        {
            var accessToken = accessTokenElement.GetString();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("auth-token", accessToken, cookieOptions);
            return Ok();
        }

        return BadRequest("Neuer Token konnte nicht extrahiert werden.");
    }


    [HttpPost]
    [Route("loginsensor")]
    public async Task<IActionResult> LoginSensor([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            return BadRequest("Username und Passwort müssen gesetzt sein.");

        var baseUrl = Environment.GetEnvironmentVariable("AUTH_API_BASE_URL");
        if (string.IsNullOrEmpty(baseUrl))
            return StatusCode(500, "AUTH_API_BASE_URL nicht gesetzt.");

        var apiUrl = $"{baseUrl.TrimEnd('/')}/auth/token/sensor";

        var response = await _httpClient.PostAsJsonAsync(apiUrl, new
        {
            username = loginRequest.Username,
            password = loginRequest.Password
        });

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Authentifizierung fehlgeschlagen.");

        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("access_token", out var accessTokenElement) &&
            root.TryGetProperty("refresh_token", out var refreshTokenElement))
        {
            var accessToken = accessTokenElement.GetString();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("auth-token", accessToken, cookieOptions);
            return Ok();
        }

        return BadRequest("Token konnte nicht extrahiert werden.");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Append("auth-token", "", new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        Response.Cookies.Append("refresh-token", "", new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        return Ok();
    }



}