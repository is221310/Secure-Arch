using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Authorize]
    [HttpGet]
    [Route("GetCustomers")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        return Ok(customers);
    }



    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest("Username und Passwort müssen gesetzt sein.");
        }

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

            return Ok(new
            {
                access_token = accessToken,
                refresh_token = refreshToken
            });
        }
        else
        {
            return BadRequest("Token konnte nicht extrahiert werden.");
        }
    }
}