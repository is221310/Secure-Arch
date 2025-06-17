using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SecureArchCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SensorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sensoren = await _context.Sensoren.ToListAsync();
            return Ok(sensoren);
        }


        [HttpGet("withCustomers")]
        public async Task<IActionResult> GetAllWithCustomers()
        {
            var sensoren = await _context.Sensoren
                .Include(s => s.Kunde)
                .ToListAsync();

            return Ok(sensoren);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Sensor neuerSensor)
        {
            if (string.IsNullOrWhiteSpace(neuerSensor.sensor_name))
                return BadRequest("Sensorname darf nicht leer sein.");

            neuerSensor.created_at = neuerSensor.created_at == default
                 ? DateTime.UtcNow
                 : DateTime.SpecifyKind(neuerSensor.created_at, DateTimeKind.Utc);

            _context.Sensoren.Add(neuerSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = neuerSensor.sensor_id }, neuerSensor);
        }


        [HttpDelete("{sensor_id:int}")]
        public async Task<IActionResult> Delete(int sensor_id)
        {
            var sensor = await _context.Sensoren.FindAsync(sensor_id);
            if (sensor == null)
                return NotFound();

            _context.Sensoren.Remove(sensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Sensor sensorUpdate)
        {
            if (id != sensorUpdate.sensor_id)
                return BadRequest("Sensor-ID stimmt nicht mit der URL überein.");

            if (string.IsNullOrWhiteSpace(sensorUpdate.sensor_name))
                return BadRequest("Sensorname darf nicht leer sein.");

            var sensor = await _context.Sensoren.FindAsync(id);
            if (sensor == null)
                return NotFound("Sensor nicht gefunden.");

            
            sensor.sensor_name = sensorUpdate.sensor_name;
            sensor.beschreibung = sensorUpdate.beschreibung;

            
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("{sensorId}/assign/{kundenId}")]
        public async Task<IActionResult> AssignSensorToKunde(int sensorId, int kundenId)
        {
            var sensor = await _context.Sensoren.FindAsync(sensorId);
            var kunde = await _context.Kunden.FindAsync(kundenId);

            if (sensor == null || kunde == null)
                return NotFound();

            sensor.kunden_id = kundenId;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("assignments")]
        public async Task<IActionResult> SaveAssignments([FromBody] List<SensorAssignmentDto> assignments)
        {
            foreach (var a in assignments)
            {
                var sensor = await _context.Sensoren.FindAsync(a.sensor_id);
                if (sensor != null)
                {
                    sensor.kunden_id = (int)a.kunden_id;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        public class SensorAssignmentDto
        {
            public int sensor_id { get; set; }
            public int? kunden_id { get; set; }
        }
    }

}
