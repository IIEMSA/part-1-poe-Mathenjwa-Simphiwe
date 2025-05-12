using System.Runtime.CompilerServices;
using CLDV6211ASSIGNMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLDV6211ASSIGNMENT.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDBcontext _context;
        public VenueController(ApplicationDBcontext context)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {
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
            return View(venue);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venues venue)
        {
            if (id != venue.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue updated successfully"
        }
        }
    }
