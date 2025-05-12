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
            var events = await _context.Eventss.Include(e ==> e.Venues).ToListAsync();
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
            if (events == null) return NotFound();

            ViewData["Events"] = _context.Venues.ToList();
            return View(events);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Events event)
        {
            if (id != event.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully"
                return RedirectToAction(nameof(Index));
            }
            ViewData["Events"]= _context.Venues.ToList();
            return View(event);
        }
//Step 1: COnfirming deletion
public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();

    var event = await _context.Eventss
        .Include(x=> x.Venues)
        .FirstOrDefaultAsync(x => x.Id == id);
    if (event == null) return NotFound();
    return View(event);

}
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var event = await _context.Eventss.FindAsync(id);
    if (events == null) return NotFound();

    var isBooked = await _context.Bookings.AnyAsync(b => b.Id == id);
    if (isBooked)
    {
        TempData["ErrorMessage"] = "Cannot delete event because it has existing bookings";
        return RedirectToAction(nameof(Index));
    }
    _context.Venues.Remove(event);
    await _context.SaveChangesAsync();
    TempData["SuccessfulMessage"] = "event deleted successfully";
    return RedirectToAction(nameof(Index));
}

pubic async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

var event = _context.Eventss.Include(s => s.Venues).FirstOrDefaultAsync(s => s.Id == id);
if (event == null) return NotFound();

return View(event);
        }
    }
}
