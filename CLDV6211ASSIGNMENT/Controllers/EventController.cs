using CLDV6211ASSIGNMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLDV6211ASSIGNMENT.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDBcontext _context;
        public EventController(ApplicationDBcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Eventss.ToListAsync();
            return View(events);
        }
        public IActionResult Create()
        {
            ViewData["Venues"]= _context.Eventss.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Venues"] = _context.Venues.ToList();
            return View(@events);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @events = await _context.Eventss.FindAsync(id);
            if (@events == null) return NotFound();

            ViewData["Events"] = _context.Venues.ToList();
            return View(events);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Events eventItem)
        {
            if (id != eventItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Events"]= _context.Venues.ToList();
            return View(eventItem);
        }
        // Step 1: Confirming deletion
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Eventss
               
                .FirstOrDefaultAsync(x => x.Id == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Eventss.FindAsync(id);
            if (eventItem == null) return NotFound();

            var isBooked = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has existing bookings";
                return RedirectToAction(nameof(Index));
            }

            _context.Eventss.Remove(eventItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Event deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Eventss
               
                .FirstOrDefaultAsync(s => s.Id == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

    }
}
