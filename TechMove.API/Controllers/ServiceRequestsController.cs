using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly TechMoveDbContext _db;

        public ServiceRequestsController(TechMoveDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (request == null) return NotFound();
            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var contract = await _db.Contracts.FindAsync(request.ContractId);
            if (contract == null) return BadRequest("Contract not found.");

            if (contract.Status == "Expired" || contract.Status == "On Hold")
                return BadRequest("Cannot raise a service request against an Expired or On Hold contract.");

            decimal exchangeRate = 18.50m;
            request.CostZAR = request.CostUSD * exchangeRate;

            _db.ServiceRequests.Add(request);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceRequest request)
        {
            if (id != request.Id) return BadRequest();

            var contract = await _db.Contracts.FindAsync(request.ContractId);
            if (contract == null || contract.Status == "Expired" || contract.Status == "On Hold")
                return BadRequest("Cannot link to an Expired or On Hold contract.");

            decimal exchangeRate = 18.50m;
            request.CostZAR = request.CostUSD * exchangeRate;

            _db.ServiceRequests.Update(request);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _db.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            _db.ServiceRequests.Remove(request);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}