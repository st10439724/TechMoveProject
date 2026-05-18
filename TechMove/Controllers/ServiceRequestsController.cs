using TechMove.Data;
using TechMove.Models;
using TechMove.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechMove.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly TechMoveDbContext _db;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(TechMoveDbContext db, CurrencyService currencyService)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
            return View(requests);
        }

        public async Task<IActionResult> Create()
        {
            // only pull active contracts - expired/on hold ones should not appear
            var activeContracts = await _db.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == "Active")
                .ToListAsync();

            if (!activeContracts.Any())
            {
                // no active contracts exist - no point showing the form
                TempData["ErrorMessage"] = "No active contracts found. Please activate a contract first.";
                return RedirectToAction("Index");
            }

            ViewBag.Contracts = new SelectList(
                activeContracts.Select(c => new {
                    Id = c.Id,
                    Display = c.Client.Name + " - " + c.ServiceLevel
                }),
                "Id",
                "Display"
            );

            ViewBag.StatusOptions = new SelectList(new[] { "Pending", "In Progress", "Completed" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            ModelState.Remove("CostZAR");
            ModelState.Remove("Contract");

            // workflow guard - check contract status before saving
            var contract = await _db.Contracts.FindAsync(request.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Selected contract does not exist.");
            }
            else if (contract.Status == "Expired")
            {
                ModelState.AddModelError("", "This contract has expired. Service requests cannot be raised against expired contracts.");
            }
            else if (contract.Status == "On Hold")
            {
                ModelState.AddModelError("", "This contract is on hold. Please reactivate it before raising a service request.");
            }

            if (!ModelState.IsValid)
            {
                var activeContracts = await _db.Contracts
                    .Include(c => c.Client)
                    .Where(c => c.Status == "Active")
                    .ToListAsync();

                ViewBag.Contracts = new SelectList(
                    activeContracts.Select(c => new {
                        Id = c.Id,
                        Display = c.Client.Name + " - " + c.ServiceLevel
                    }),
                    "Id",
                    "Display"
                );

                ViewBag.StatusOptions = new SelectList(new[] { "Pending", "In Progress", "Completed" });
                return View(request);
            }

            // calculate ZAR - hardcoded rate for now, live rate comes in Step 4
            decimal exchangeRate = 18.50m;
            request.CostZAR = request.CostUSD * exchangeRate;

            _db.ServiceRequests.Add(request);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = await _db.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            var activeContracts = await _db.Contracts
                .Where(c => c.Status == "Active")
                .Include(c => c.Client)
                .ToListAsync();

            ViewBag.Contracts = new SelectList(
                activeContracts.Select(c => new {
                    Id = c.Id,
                    Display = c.Client.Name + " - " + c.ServiceLevel
                }),
                "Id",
                "Display",
                request.ContractId
            );

            ViewBag.StatusOptions = new SelectList(new[] { "Pending", "In Progress", "Completed" }, request.Status);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ServiceRequest request)
        {
            if (id != request.Id) return NotFound();

            ModelState.Remove("CostZAR");
            ModelState.Remove("Contract");

            // run the same workflow guard on edit too
            var contract = await _db.Contracts.FindAsync(request.ContractId);

            if (contract == null || contract.Status == "Expired" || contract.Status == "On Hold")
            {
                ModelState.AddModelError("", "Cannot link a service request to an Expired or On Hold contract.");
            }

            if (!ModelState.IsValid)
            {
                var activeContracts = await _db.Contracts
                    .Include(c => c.Client)
                    .Where(c => c.Status == "Active")
                    .ToListAsync();

                ViewBag.Contracts = new SelectList(
                    activeContracts.Select(c => new {
                        Id = c.Id,
                        Display = c.Client.Name + " - " + c.ServiceLevel
                    }),
                    "Id",
                    "Display",
                    request.ContractId
                );

                ViewBag.StatusOptions = new SelectList(new[] { "Pending", "In Progress", "Completed" }, request.Status);
                return View(request);
            }

            // recalculate ZAR in case USD changed
            decimal exchangeRate = 18.50m;
            request.CostZAR = request.CostUSD * exchangeRate;

            _db.ServiceRequests.Update(request);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _db.ServiceRequests.FindAsync(id);
            _db.ServiceRequests.Remove(request);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (request == null) return NotFound();
            return View(request);
        }
    }
}