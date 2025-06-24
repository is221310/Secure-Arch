using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using SecureArchCore.Models;


namespace SecureArchCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        public class UserCreateDto
        {
            public string firstname { get; set; } = string.Empty;
            public string lastname { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
            public string telephone { get; set; } = string.Empty;
            public string role { get; set; } = string.Empty;
            public string address { get; set; } = string.Empty;
            public int? kunden_id { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllWithCustomers()
        {
            var users = await _context.Users
                .Include(u => u.Kunde) // lädt den Kunden mit
                .Select(u => new User
                {
                    id = u.id,
                    firstname = u.firstname,
                    lastname = u.lastname,
                    email = u.email,
                    telephone = u.telephone,
                    role = u.role,
                    address = u.address,
                    kunden_id = u.kunden_id,
                   
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users
                .Where(u => u.id == id)
                .Select(u => new User
                {
                    id = u.id,
                    firstname = u.firstname,
                    lastname = u.lastname,
                    email = u.email,
                    telephone = u.telephone,
                    role = u.role,
                    address = u.address,
                    kunden_id = u.kunden_id
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }



        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.email) || string.IsNullOrWhiteSpace(dto.password))
                return BadRequest("Email und Passwort sind erforderlich.");

            if (await _context.Users.AnyAsync(u => u.email == dto.email))
                return BadRequest("Ein Benutzer mit dieser E-Mail existiert bereits.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            var user = new User
            {
                firstname = dto.firstname,
                lastname = dto.lastname,
                email = dto.email,
                password = hashedPassword,
                telephone = dto.telephone,
                role = dto.role,
                address = dto.address,
                created = DateTime.UtcNow,
                kunden_id = dto.kunden_id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Benutzer erfolgreich erstellt.", user.email });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User dto)
        {
            if (id != dto.id)
                return BadRequest("Die ID in der URL stimmt nicht mit dem Payload überein.");

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Benutzer nicht gefunden.");

            if (string.IsNullOrWhiteSpace(dto.firstname) || string.IsNullOrWhiteSpace(dto.email))
                return BadRequest("Pflichtfelder fehlen.");

            // Felder aktualisieren
            user.firstname = dto.firstname;
            user.lastname = dto.lastname;
            user.email = dto.email;
            user.role = dto.role;
            user.telephone = dto.telephone;
            user.address = dto.address;
            user.kunden_id = dto.kunden_id;

            // Nur neues Passwort setzen, wenn übergeben
            if (!string.IsNullOrWhiteSpace(dto.password))
            {
                user.password = BCrypt.Net.BCrypt.HashPassword(dto.password);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}