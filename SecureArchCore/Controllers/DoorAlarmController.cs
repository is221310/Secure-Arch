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
        if (request == null || request.door_opened?.ToLower() != "true")
            return BadRequest("Kein gültiges Tür-Alarm-Event.");

        // Sensorname aus dem 'sub'-Claim holen
        var sensorName = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrWhiteSpace(sensorName))
            return Unauthorized("Sensorname nicht im Token enthalten (sub-Claim).");

        // Sensor + zugehörigen Kunden aus der DB holen
        var sensor = await _context.Sensoren
            .Include(s => s.Kunde)
            .FirstOrDefaultAsync(s => s.sensor_name == sensorName);

        if (sensor == null)
            return NotFound($"Sensor '{sensorName}' nicht gefunden.");

        if (sensor.Kunde == null)
            return NotFound("Sensor ist keinem Kunden zugeordnet.");

        // Customer ID für Zammad: z. B. Kunden-E-Mail oder Firmenname
        var customerId = sensor.Kunde.kunden_name ?? sensor.Kunde.kunden_name ?? "unbekannt";

        // Payload für Zammad-Ticket
        var zammadPayload = new
        {
            title = "Türalarm ausgelöst",
            group = "Users",
            customer_id = "1",
            article = new
            {
                subject = "Tür geöffnet",
                body = $"Der Sensor '{sensorName}' hat einen Türalarm gemeldet.",
                type = "email",
                @internal = false  // @ für C#-Schlüsselwort "internal"
            }
        };

        var json = JsonSerializer.Serialize(zammadPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("nicole.braun@zammad.org:12345"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var zammadApiUrl = "http://localhost:8081/api/v1/tickets";
        var response = await httpClient.PostAsync(zammadApiUrl, content);

        if (response.IsSuccessStatusCode)
            return Ok("Ticket erfolgreich erstellt.");

        var error = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, $"Fehler bei Ticket-Erstellung: {error}");
    }

public class DoorAlarmRequest
    {
        public string? door_opened { get; set; }
    }
}
