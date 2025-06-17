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
            var sensoren = await _context.Sensoren
                .Include(s => s.Kunde)
                .ToListAsync();

            return Ok(sensoren);
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
