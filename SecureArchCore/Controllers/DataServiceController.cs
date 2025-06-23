using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureArchCore.Models;

namespace SecureArchCore.Controllers
{

        [ApiController]
        [Route("[controller]")]
        public class DataServiceController : ControllerBase
        {
            private readonly AppDbContext _context;
            public DataServiceController(AppDbContext context)
            {
                _context = context;
            }


            [HttpGet("getConfig/{sensorName}")]
            public async Task<IActionResult> GetConfig(string sensorName)
            {
                var sensor = await _context.Sensoren
                    .FirstOrDefaultAsync(s => s.sensor_name == sensorName);

                if (sensor == null)
                    return NotFound($"Sensor mit Name '{sensorName}' nicht gefunden.");

                var ipList = sensor.ip_addresses ?? new List<string>();
                return Ok(ipList);
            }

            [HttpGet("temperatur")]
            public async Task<IActionResult> GetAllTemperaturen()
            {
                var daten = await _context.Temperaturen
                    .Include(t => t.Sensor)
                    .OrderByDescending(t => t.timestamp)
                    .ToListAsync();

                return Ok(daten);
            }

            [HttpGet("temperatur/sensor/{sensorId}")]
            public async Task<IActionResult> GetTemperaturenBySensor(int sensorId)
            {
                var daten = await _context.Temperaturen
                    .Where(t => t.sensor_id == sensorId)
                    .OrderByDescending(t => t.timestamp)
                    .ToListAsync();

                return Ok(daten);
            }


        [HttpGet("temperaturen/by-user/")]
        [Authorize]
        public async Task<IActionResult> GetTemperaturen()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == username);
            if (user == null)
                return NotFound("User nicht gefunden.");

            var kundenId = user.kunden_id;

            var temperaturen = await _context.Temperaturen
                .Include(t => t.Sensor)
                .Where(t => t.Sensor.kunden_id == kundenId)
                .ToListAsync();

            return Ok(temperaturen);
        }

        public class TemperaturCreateByNameDto
        {
            public string sensor_name { get; set; } = string.Empty;
            public double temperatur { get; set; }
            public DateTime timestamp { get; set; } = DateTime.UtcNow;
        }

        [HttpPost("temperatur")]
        public async Task<IActionResult> PostTemperaturByName([FromBody] TemperaturCreateByNameDto dto)
        {
            var sensor = await _context.Sensoren
                .FirstOrDefaultAsync(s => s.sensor_name == dto.sensor_name);

            if (sensor == null)
            {
                return NotFound($"Sensor mit Name '{dto.sensor_name}' nicht gefunden.");
            }

            var temperatur = new Temperatur
            {
                sensor_id = sensor.sensor_id,
                temperatur = dto.temperatur,
                timestamp = dto.timestamp
            };

            _context.Temperaturen.Add(temperatur);
            await _context.SaveChangesAsync();

            return Ok(temperatur);
        }


        [HttpGet("ipresults")]
            public async Task<IActionResult> GetAllIpResults()
            {
                var daten = await _context.IpResults
                    .Include(r => r.Sensor)
                    .OrderByDescending(r => r.timestamp)
                    .ToListAsync();

                return Ok(daten);
            }

            [HttpGet("ipresults/sensor/{sensorId}")]
            public async Task<IActionResult> GetIpResultsBySensor(int sensorId)
            {
                var daten = await _context.IpResults
                    .Where(r => r.sensor_id == sensorId)
                    .OrderByDescending(r => r.timestamp)
                    .ToListAsync();

                return Ok(daten);
            }

        [Authorize]
        [HttpGet("ipresults/by-user/")]
        public async Task<IActionResult> GetIpResultsByUsername()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == username);
            if (user == null)
                return NotFound("User nicht gefunden.");

            if (user == null || user.kunden_id == null)
                return NotFound("Benutzer oder Kunde nicht gefunden.");

            // Alle IP-Results für Sensoren des Kunden
            var ipResults = await _context.IpResults
                .AsNoTracking()
                .Where(ip => _context.Sensoren
                    .Where(s => s.kunden_id == user.kunden_id)
                    .Select(s => s.sensor_id)
                    .Contains(ip.sensor_id))
                .ToListAsync();

            return Ok(ipResults);
        }
        public class IpResultCreateByNameDto
        {
            public string sensor_name { get; set; } = string.Empty;
            public string ip_address { get; set; } = string.Empty;
            public bool status { get; set; }
            public DateTime timestamp { get; set; } = DateTime.UtcNow;
        }

        [HttpPost("ipresults")]
        public async Task<IActionResult> PostIpResultsBySensor([FromBody] Dictionary<string, string> ipStatusMap)
        {
            // 1. Sensorname aus 'sub' Claim auslesen
            var sensorName =  User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(sensorName))
                return Unauthorized("Kein 'sub'-Claim im JWT.");

            // 2. Sensor aus DB laden
            var sensor = await _context.Sensoren.FirstOrDefaultAsync(s => s.sensor_name == sensorName);
            if (sensor == null)
                return NotFound($"Sensor mit Name '{sensorName}' nicht gefunden.");

            // 3. Neue IpResults für jede IP im Dictionary erstellen
            var now = DateTime.UtcNow;
            var ipResults = new List<IpResult>();

            foreach (var kvp in ipStatusMap)
            {
                bool status = false;
                if (!bool.TryParse(kvp.Value, out status))
                {
                    // Optional: Fehler für ungültigen Status-Wert
                    return BadRequest($"Ungültiger Status-Wert für IP '{kvp.Key}': '{kvp.Value}'. Erwartet 'true' oder 'false'.");
                }

                ipResults.Add(new IpResult
                {
                    sensor_id = sensor.sensor_id,
                    ip_address = kvp.Key,
                    status = status,
                    timestamp = now
                });
            }

            // 4. Speichern in DB
            _context.IpResults.AddRange(ipResults);
            await _context.SaveChangesAsync();

            return Ok(ipResults);
        }

        public class IpResultCreateDto
        {
            public int sensor_id { get; set; }
            public string ip_address { get; set; } = string.Empty;
            public bool status { get; set; }
        }

    }
}
