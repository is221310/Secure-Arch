using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kunden = await _context.Kunden.ToListAsync();
            return Ok(kunden);
        }
    }
}
