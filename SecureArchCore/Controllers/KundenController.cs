using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureArchCore.Models;


namespace SecureArchCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KundeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KundeController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kunden = await _context.Kunden.ToListAsync();
            return Ok(kunden);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Kunde kundeDto)
        {
            if (id != kundeDto.kunden_id)
                return BadRequest("ID stimmt nicht mit Kunde überein.");

            var kunde = await _context.Kunden.FindAsync(id);
            if (kunde == null)
                return NotFound("Kunde nicht gefunden.");

            kunde.kunden_name = kundeDto.kunden_name;
            kunde.created_at = kundeDto.created_at;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Kunde neuerKunde)
        {
            if (string.IsNullOrWhiteSpace(neuerKunde.kunden_name))
                return BadRequest("Kundenname darf nicht leer sein.");

            if (neuerKunde.created_at == default)
                neuerKunde.created_at = DateTime.UtcNow;

            neuerKunde.Users ??= new List<User>();
            neuerKunde.Sensoren ??= new List<Sensor>();

            _context.Kunden.Add(neuerKunde);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = neuerKunde.kunden_id }, neuerKunde);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{kundenId:int}")]
        public async Task<IActionResult> Delete(int kundenId)
        {
            var kunde = await _context.Kunden.FindAsync(kundenId);
            if (kunde == null)
                return NotFound();

            _context.Kunden.Remove(kunde);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}
