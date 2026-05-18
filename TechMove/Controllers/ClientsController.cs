using TechMove.Data;
using TechMove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace TechMove.Controllers
{

    public class ClientsController : Controller
    {
        private readonly TechMoveDbContext _db;

        public ClientsController(TechMoveDbContext db)
        {
            _db = db;
        }

        // show all clients //doesnt work??
        public async Task<IActionResult> Index()
        {
            var clients = await _db.Clients.ToListAsync();
            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // load the client we want to edit
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(client);

            _db.Clients.Update(client);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // just to view client details and their contracts
        public async Task<IActionResult> Details(int id)
        {
            var client = await _db.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return NotFound();
            return View(client);
        }
    }

}
