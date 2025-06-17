using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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



        //TODO weiterre API Request an Auth
        [HttpPost]
        public async Task<IActionResult> Create(User dto)
        {
            var user = new User
            {
                firstname = dto.firstname,
                lastname = dto.lastname,
                email = dto.email,
                telephone = dto.telephone,
                role = dto.role,
                address = dto.address,
                kunden_id = dto.kunden_id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            dto.id = user.id; 

            return CreatedAtAction(nameof(GetById), new { id = user.id }, dto);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User dto)
        {
            if (id != dto.id)
                return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.firstname = dto.firstname;
            user.lastname = dto.lastname;
            user.email = dto.email;
            user.telephone = dto.telephone;
            user.role = dto.role;
            user.address = dto.address;
            user.kunden_id = dto.kunden_id;

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