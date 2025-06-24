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

        public class TemperaturCreateDto
        {
            public double Temperatur { get; set; }
            public DateTime Timestamp { get; set; } = default;
        }

        public class TemperaturDto
        {
            public int id { get; set; }
            public int sensor_id { get; set; }
            public string sensor_name { get; set; } = string.Empty;
            public double temperatur { get; set; }
            public DateTime timestamp { get; set; }
        }

        public class IpResultDto
        {
            public int id { get; set; }
            public int sensor_id { get; set; }
            public string sensor_name { get; set; } = string.Empty;
            public string ip_address { get; set; } = string.Empty;
            public bool result { get; set; }
            public DateTime timestamp { get; set; }
        }

        public class IpResultCreateDto
        {
            public int sensor_id { get; set; }
            public string ip_address { get; set; } = string.Empty;
            public bool status { get; set; }
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
            var username = User.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                ?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.email == username);

            if (user?.kunden_id == null)
                return NotFound("Benutzer oder Kunde nicht gefunden.");

            var temperaturen = await _context.Temperaturen
                .AsNoTracking()
                .Where(t => _context.Sensoren
                    .Where(s => s.kunden_id == user.kunden_id)
                    .Select(s => s.sensor_id)
                    .Contains(t.sensor_id))
                .Join(
                    _context.Sensoren,
                    t => t.sensor_id,
                    s => s.sensor_id,
                    (t, s) => new TemperaturDto
                    {
                        id = t.id,
                        sensor_id = t.sensor_id,
                        sensor_name = s.sensor_name,
                        temperatur = t.temperatur,
                        timestamp = t.timestamp
                    })
                .ToListAsync();

            return Ok(temperaturen);
        }


        [Authorize]
        [HttpPost("temperatur")]
        public async Task<IActionResult> PostTemperaturBySensor([FromBody] TemperaturCreateDto dto)
        {
            // Sensorname aus JWT 'sub'-Claim lesen
            var sensorName = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(sensorName))
                return Unauthorized("Kein 'sub'-Claim im JWT.");

            // Sensor aus der DB laden
            var sensor = await _context.Sensoren.FirstOrDefaultAsync(s => s.sensor_name == sensorName);
            if (sensor == null)
                return NotFound($"Sensor mit Name '{sensorName}' nicht gefunden.");

            var temperatur = new Temperatur
            {
                sensor_id = sensor.sensor_id,
                temperatur = dto.Temperatur,
                timestamp = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp
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
            var username = User.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                ?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.email == username);

            if (user?.kunden_id == null)
                return NotFound("Benutzer oder Kunde nicht gefunden.");

            var ipResults = await _context.IpResults
                .AsNoTracking()
                .Where(ip => _context.Sensoren
                    .Where(s => s.kunden_id == user.kunden_id)
                    .Select(s => s.sensor_id)
                    .Contains(ip.sensor_id))
                .Join(
                    _context.Sensoren,
                    ip => ip.sensor_id,
                    sensor => sensor.sensor_id,
                    (ip, sensor) => new IpResultDto
                    {
                        id = ip.id,
                        sensor_id = ip.sensor_id,
                        sensor_name = sensor.sensor_name,
                        ip_address = ip.ip_address,
                        result = ip.status,
                        timestamp = ip.timestamp
                    })
                .ToListAsync();

            return Ok(ipResults);
        }



    }
}
