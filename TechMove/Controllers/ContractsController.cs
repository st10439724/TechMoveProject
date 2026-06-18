//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using TechMove.Data;
//using TechMove.Models;

//namespace TechMove.Controllers
//{
//    public class ContractsController : Controller
//    {
//        private readonly TechMoveDbContext _db;
//        private readonly IWebHostEnvironment _env;


//        public ContractsController(TechMoveDbContext db, IWebHostEnvironment env)
//        {
//            _db = db;
//            _env = env;
//        }

//        public async Task<IActionResult> Index(string status, DateTime? startDate, DateTime? endDate)
//        {
//            var query = _db.Contracts
//                .Include(c => c.Client)
//                .AsQueryable();

//            if (!string.IsNullOrEmpty(status))
//                query = query.Where(c => c.Status == status);

//            if (startDate.HasValue)
//                query = query.Where(c => c.StartDate >= startDate.Value);

//            if (endDate.HasValue)
//                query = query.Where(c => c.EndDate <= endDate.Value);

//            ViewBag.SelectedStatus = status;
//            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
//            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
//            ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" }, status);

//            return View(await query.ToListAsync());
//        }

//        public async Task<IActionResult> Create()
//        {
//            ViewBag.Clients = new SelectList(await _db.Clients.ToListAsync(), "Id", "Name");
//            ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" });
//            ViewBag.ServiceLevels = new SelectList(new[] { "Gold", "Silver", "Bronze" });
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(Contract contract, IFormFile? pdfFile)
//        {
//            ModelState.Remove("SignedAgreementPath");
//            ModelState.Remove("Client");
//            ModelState.Remove("ServiceRequests");

//            if (!ModelState.IsValid)
//            {
//                ViewBag.Clients = new SelectList(await _db.Clients.ToListAsync(), "Id", "Name");
//                ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" });
//                ViewBag.ServiceLevels = new SelectList(new[] { "Gold", "Silver", "Bronze" });
//                return View(contract);
//            }

//            // handle the PDF upload if one was provided
//            if (pdfFile != null && pdfFile.Length > 0)
//            {
//                // only allow PDF files - reject anything else
//                var extension = Path.GetExtension(pdfFile.FileName).ToLower();
//                if (extension != ".pdf")
//                {
//                    ModelState.AddModelError("", "Only PDF files are allowed for signed agreements.");
//                    ViewBag.Clients = new SelectList(await _db.Clients.ToListAsync(), "Id", "Name");
//                    ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" });
//                    ViewBag.ServiceLevels = new SelectList(new[] { "Gold", "Silver", "Bronze" });
//                    return View(contract);
//                }

//                // give the file a unique name so uploads don't overwrite each other
//                var uniqueFileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
//                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
//                var filePath = Path.Combine(uploadsFolder, uniqueFileName);


//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await pdfFile.CopyToAsync(stream);
//                }

//                contract.SignedAgreementPath = uniqueFileName;
//            }

//            _db.Contracts.Add(contract);
//            await _db.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        public async Task<IActionResult> Edit(int id)
//        {
//            var contract = await _db.Contracts.FindAsync(id);
//            if (contract == null) return NotFound();

//            ViewBag.Clients = new SelectList(await _db.Clients.ToListAsync(), "Id", "Name", contract.ClientId);
//            ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" }, contract.Status);
//            ViewBag.ServiceLevels = new SelectList(new[] { "Gold", "Silver", "Bronze" }, contract.ServiceLevel);
//            return View(contract);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? pdfFile)
//        {
//            if (id != contract.Id) return NotFound();

//            ModelState.Remove("SignedAgreementPath");
//            ModelState.Remove("Client");
//            ModelState.Remove("ServiceRequests");

//            if (!ModelState.IsValid)
//            {
//                ViewBag.Clients = new SelectList(await _db.Clients.ToListAsync(), "Id", "Name", contract.ClientId);
//                ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" }, contract.Status);
//                ViewBag.ServiceLevels = new SelectList(new[] { "Gold", "Silver", "Bronze" }, contract.ServiceLevel);
//                return View(contract);
//            }

//            // if a new PDF was uploaded replace the old one
//            if (pdfFile != null && pdfFile.Length > 0)
//            {
//                var extension = Path.GetExtension(pdfFile.FileName).ToLower();
//                if (extension != ".pdf")
//                {
//                    ModelState.AddModelError("", "Only PDF files are allowed for signed agreements.");
//                    ViewBag.Clients = new SelectList(await _db.Clients.ToListAsync(), "Id", "Name", contract.ClientId);
//                    ViewBag.StatusOptions = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" }, contract.Status);
//                    ViewBag.ServiceLevels = new SelectList(new[] { "Gold", "Silver", "Bronze" }, contract.ServiceLevel);
//                    return View(contract);
//                }

//                var uniqueFileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
//                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
//                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await pdfFile.CopyToAsync(stream);
//                }

//                contract.SignedAgreementPath = uniqueFileName;
//            }
//            else
//            {
//                // no new file uploaded - keep the existing file path
//                var existing = await _db.Contracts.AsNoTracking()
//                    .FirstOrDefaultAsync(c => c.Id == id);
//                contract.SignedAgreementPath = existing?.SignedAgreementPath;
//            }

//            _db.Contracts.Update(contract);
//            await _db.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        public async Task<IActionResult> Delete(int id)
//        {
//            var contract = await _db.Contracts
//                .Include(c => c.Client)
//                .FirstOrDefaultAsync(c => c.Id == id);

//            if (contract == null) return NotFound();
//            return View(contract);
//        }

//        [HttpPost, ActionName("Delete")]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var contract = await _db.Contracts.FindAsync(id);
//            _db.Contracts.Remove(contract);
//            await _db.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        public async Task<IActionResult> Details(int id)
//        {
//            var contract = await _db.Contracts
//                .Include(c => c.Client)
//                .Include(c => c.ServiceRequests)
//                .FirstOrDefaultAsync(c => c.Id == id);

//            if (contract == null) return NotFound();
//            return View(contract);
//        }

//        // download the signed agreement PDF(Part 3 POE to be done)
//        public IActionResult DownloadAgreement(int id)
//        {
//            var contract = _db.Contracts.Find(id);
//            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
//            {
//                TempData["ErrorMessage"] = "No signed agreement found for this contract.";
//                return RedirectToAction("Details", new { id });
//            }

//            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
//            var filePath = Path.Combine(uploadsFolder, contract.SignedAgreementPath);

//            if (!System.IO.File.Exists(filePath))
//            {
//                TempData["ErrorMessage"] = "File not found on server.";
//                return RedirectToAction("Details", new { id });
//            }

//            // send the file back to the browser as a download
//            var fileBytes = System.IO.File.ReadAllBytes(filePath);
//            return File(fileBytes, "application/pdf", contract.SignedAgreementPath);
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _env;

        public ContractsController(ApiService apiService, IWebHostEnvironment env)
        {
            _apiService = apiService;
            _env = env;
        }

        public async Task<IActionResult> Index(
            string? status, DateTime? startDate, DateTime? endDate)
        {
            var contracts = await _apiService.GetContractsAsync(
                status, startDate, endDate);

            ViewBag.SelectedStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.StatusOptions = new SelectList(
                new[] { "Draft", "Active", "Expired", "On Hold" }, status);

            return View(contracts);
        }

        public async Task<IActionResult> Create()
        {
            var clients = await _apiService.GetClientsAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
            ViewBag.StatusOptions = new SelectList(
                new[] { "Draft", "Active", "Expired", "On Hold" });
            ViewBag.ServiceLevels = new SelectList(
                new[] { "Gold", "Silver", "Bronze" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Contract contract, IFormFile? pdfFile)
        {
            ModelState.Remove("SignedAgreementPath");
            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");

            if (!ModelState.IsValid)
            {
                var clients = await _apiService.GetClientsAsync();
                ViewBag.Clients = new SelectList(clients, "Id", "Name");
                ViewBag.StatusOptions = new SelectList(
                    new[] { "Draft", "Active", "Expired", "On Hold" });
                ViewBag.ServiceLevels = new SelectList(
                    new[] { "Gold", "Silver", "Bronze" });
                return View(contract);
            }

            // handle the PDF upload if one was provided
            if (pdfFile != null && pdfFile.Length > 0)
            {
                // only allow PDF files - reject anything else
                var extension = Path.GetExtension(pdfFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("",
                        "Only PDF files are allowed for signed agreements.");
                    var clients = await _apiService.GetClientsAsync();
                    ViewBag.Clients = new SelectList(clients, "Id", "Name");
                    ViewBag.StatusOptions = new SelectList(
                        new[] { "Draft", "Active", "Expired", "On Hold" });
                    ViewBag.ServiceLevels = new SelectList(
                        new[] { "Gold", "Silver", "Bronze" });
                    return View(contract);
                }

                // give the file a unique name so uploads dont overwrite each other
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                contract.SignedAgreementPath = uniqueFileName;
            }

            await _apiService.CreateContractAsync(contract);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();

            var clients = await _apiService.GetClientsAsync();
            ViewBag.Clients = new SelectList(
                clients, "Id", "Name", contract.ClientId);
            ViewBag.StatusOptions = new SelectList(
                new[] { "Draft", "Active", "Expired", "On Hold" }, contract.Status);
            ViewBag.ServiceLevels = new SelectList(
                new[] { "Gold", "Silver", "Bronze" }, contract.ServiceLevel);
            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id, Contract contract, IFormFile? pdfFile)
        {
            if (id != contract.Id) return NotFound();

            ModelState.Remove("SignedAgreementPath");
            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");

            if (!ModelState.IsValid)
            {
                var clients = await _apiService.GetClientsAsync();
                ViewBag.Clients = new SelectList(
                    clients, "Id", "Name", contract.ClientId);
                ViewBag.StatusOptions = new SelectList(
                    new[] { "Draft", "Active", "Expired", "On Hold" }, contract.Status);
                ViewBag.ServiceLevels = new SelectList(
                    new[] { "Gold", "Silver", "Bronze" }, contract.ServiceLevel);
                return View(contract);
            }

            // if a new PDF was uploaded replace the old one
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var extension = Path.GetExtension(pdfFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("",
                        "Only PDF files are allowed for signed agreements.");
                    var clients = await _apiService.GetClientsAsync();
                    ViewBag.Clients = new SelectList(
                        clients, "Id", "Name", contract.ClientId);
                    ViewBag.StatusOptions = new SelectList(
                        new[] { "Draft", "Active", "Expired", "On Hold" }, contract.Status);
                    ViewBag.ServiceLevels = new SelectList(
                        new[] { "Gold", "Silver", "Bronze" }, contract.ServiceLevel);
                    return View(contract);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                contract.SignedAgreementPath = uniqueFileName;
            }
            else
            {
                // no new file uploaded - keep the existing file path
                var existing = await _apiService.GetContractAsync(id);
                contract.SignedAgreementPath = existing?.SignedAgreementPath;
            }

            await _apiService.UpdateContractAsync(contract);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteContractAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // download the signed agreement PDF
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
            {
                TempData["ErrorMessage"] = "No signed agreement found for this contract.";
                return RedirectToAction("Details", new { id });
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, contract.SignedAgreementPath);

            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "File not found on server.";
                return RedirectToAction("Details", new { id });
            }

            // send the file back to the browser as a download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", contract.SignedAgreementPath);
        }
    }
}
