using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityArch.Models;
using System;

namespace SecurityArch.Controllers;

[ApiController]
[Route("[controller]")]
public class CoreServiceController : ControllerBase
{
    private readonly AppDbContext _context;


    public CoreServiceController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Route("GetCustomers")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        return Ok(customers);
    }

}
