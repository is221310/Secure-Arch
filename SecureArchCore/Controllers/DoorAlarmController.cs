using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("DataService")]
public class DoorAlarmController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoorAlarmController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("door_alarm")]
    public async Task<IActionResult> DoorAlarm([FromBody] DoorAlarmRequest request)
    {
        Console.WriteLine("Empfangene Anfrage für Türalarm.");

        if (request == null)
        {
            Console.WriteLine("Request ist null.");
            return BadRequest("Kein gültiges Tür-Alarm-Event.");
        }

        Console.WriteLine($"Tür geöffnet: {request.door_opened}");

        if (request.door_opened?.ToLower() != "true")
        {
            Console.WriteLine("Tür ist nicht geöffnet.");
            return BadRequest("Kein gültiges Tür-Alarm-Event.");
        }

        var sensorName = User.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        Console.WriteLine($"Sensorname aus Token: {sensorName}");

        if (string.IsNullOrWhiteSpace(sensorName))
        {
            Console.WriteLine("Sensorname ist leer oder null.");
            return Unauthorized("Sensorname nicht im Token enthalten (sub-Claim).");
        }

        var sensor = await _context.Sensoren
            .Include(s => s.Kunde)
            .FirstOrDefaultAsync(s => s.sensor_name == sensorName);

        if (sensor == null)
        {
            Console.WriteLine($"Sensor '{sensorName}' nicht gefunden.");
            return NotFound($"Sensor '{sensorName}' nicht gefunden.");
        }

        if (sensor.Kunde == null)
        {
            Console.WriteLine("Sensor ist keinem Kunden zugeordnet.");
            return NotFound("Sensor ist keinem Kunden zugeordnet.");
        }

        var kundenId = sensor.kunden_id;
        Console.WriteLine($"Kunden-ID des Sensors: {kundenId}");

        var users = await _context.Users
            .Where(u => u.kunden_id == kundenId && !string.IsNullOrEmpty(u.email))
            .ToListAsync();

        Console.WriteLine($"Gefundene Benutzer: {users.Count}");

        if (users.Count == 0)
        {
            Console.WriteLine("Keine Benutzer mit passender Kunden-ID gefunden.");
            return NotFound("Keine Benutzer mit passender Kunden-ID gefunden.");
        }

        var zammadBaseUrl = Environment.GetEnvironmentVariable("ZAMMAD_API_URL") ?? "http://localhost:8081";
        var zammadUsername = Environment.GetEnvironmentVariable("ZAMMAD_USERNAME") ?? "nicole.braun@zammad.org";
        var zammadPassword = Environment.GetEnvironmentVariable("ZAMMAD_PASSWORD") ?? "12345";

        Console.WriteLine($"Zammad-URL: {zammadBaseUrl}");
        Console.WriteLine($"Zammad-Benutzer: {zammadUsername}");

        var errors = new List<string>();

        using var httpClient = new HttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{zammadUsername}:{zammadPassword}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        foreach (var user in users)
        {
            Console.WriteLine($"Erstelle Ticket für Benutzer: {user.email}");

            var zammadPayload = new
            {
                title = "Türalarm ausgelöst",
                group = "Users",
                customer = user.email,
                article = new
                {
                    subject = "Tür geöffnet",
                    body = $"Der Sensor '{sensorName}' hat einen Türalarm gemeldet.",
                    type = "note",
                    @internal = false
                }
            };

            var json = JsonSerializer.Serialize(zammadPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{zammadBaseUrl}/api/v1/tickets", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fehler beim Erstellen des Tickets für {user.email}: {errorText}");
                errors.Add($"Fehler für {user.email}: {errorText}");
            }
            else
            {
                Console.WriteLine($"Ticket erfolgreich erstellt für {user.email}");
            }
        }

        if (errors.Any())
        {
            Console.WriteLine("Fehler beim Erstellen einiger Tickets:");
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
            return StatusCode(500, $"Einige Tickets konnten nicht erstellt werden:\n{string.Join("\n", errors)}");
        }

        Console.WriteLine("Tickets für alle Benutzer erfolgreich erstellt.");
        return Ok("Tickets für alle Benutzer erfolgreich erstellt.");
    }

    public class DoorAlarmRequest
    {
        public string? door_opened { get; set; }
    }
}
