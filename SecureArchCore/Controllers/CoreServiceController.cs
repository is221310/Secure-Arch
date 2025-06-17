using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureArchCore.Models;
using SecurityArch.Models;
using System;
using System.Net.Http;
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
        {
            return BadRequest("Username und Passwort müssen gesetzt sein.");
        }

        //TODO aus Env holen 
        var apiUrl = "http://localhost:8000/auth/token";

        var response = await _httpClient.PostAsJsonAsync(apiUrl, new
        {
            username = loginRequest.Username,
            password = loginRequest.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Authentifizierung fehlgeschlagen.");
        }
        var jsonString = await response.Content.ReadAsStringAsync();

        using var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("access_token", out JsonElement accessTokenElement) &&
            root.TryGetProperty("refresh_token", out JsonElement refreshTokenElement))
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

            return Ok();
        }
        else
        {
            return BadRequest("Token konnte nicht extrahiert werden.");
        }
    }
}