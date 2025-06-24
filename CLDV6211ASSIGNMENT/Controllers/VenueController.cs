using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Azure.Core;
using CLDV6211ASSIGNMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace CLDV6211ASSIGNMENT.Controllers
{
    public class VenueController : Controller
    {
        
        private readonly ApplicationDBcontext _context;
        private readonly IConfiguration _configuration;

        public VenueController(ApplicationDBcontext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
            


        public async Task<IActionResult> Create(Venues venue)
        {
            ViewBag.EventTypeList = new SelectList(_context.EventTypes, "EventTypeID", "Name");
          

            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                    venue.ImageUrl = blobUrl;

                }
                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            ViewBag.EventTypeList = new SelectList(_context.EventTypes, "EventTypeID", "Name", venue.EventTypeID);
            return View(venue);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venues venue)
        {
            if (id != venue.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                    venue.ImageUrl = blobUrl;

                }
                _context.Update(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
        //Step 1: confirm deletion
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(x => x.Id == id);
            if (venue == null) return NotFound();
            return View(venue);

        }

        //Step 2: Perform deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete venue because it has existing bookings";
                return RedirectToAction(nameof(Index));
            }
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessfulMessage"] = "Venue deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(s => s.Id == id);
            if (venue == null) return NotFound();

            return View(venue);
        }
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            var containerName = _configuration["AzureBlobStorage:ContainerName"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        public async Task<IActionResult> Search(string eventType, bool? isAvailable)
        {
            var venues = _context.Venues.Include(v => v.EventType).AsQueryable();

            if (!string.IsNullOrEmpty(eventType))
            {
                venues = venues.Where(v => v.EventType.name == eventType);
            }

            if (isAvailable.HasValue)
            {
                venues = venues.Where(v => v.IsAvailable == isAvailable.Value);
            }

            return View("Index", await venues.ToListAsync());
        }
        

        


    }
}
