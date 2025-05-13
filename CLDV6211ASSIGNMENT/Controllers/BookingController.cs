using CLDV6211ASSIGNMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLDV6211ASSIGNMENT.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDBcontext _context;
        public BookingController(ApplicationDBcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(i => i.Venue)
                .Include(i => i.Event)
                .ToListAsync();
            return View(bookings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bookings bookings)
        {
            var selectedEvent = await _context.Eventss.FirstOrDefaultAsync(e => e.Id == bookings.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["Events"] = _context.Eventss.ToList();
                ViewData["Venues"] = _context.Venues.ToList();
                return View(bookings);
            }
            var conflict = await _context.Bookings
               .Include(b => b.Event)
               .AnyAsync(b => b.Id == bookings.Id &&
                              b.Event.EventDate.Date == selectedEvent.EventDate.Date);
            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewData["Events"] = _context.Eventss.ToList();
                ViewData["Venues"] = _context.Venues.ToList();
                return View(bookings);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(bookings);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewData["Events"] = _context.Eventss.ToList();
                    ViewData["Venues"] = _context.Venues.ToList();
                    return View(bookings);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(bookings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Events"] = _context.Eventss.ToList();
            ViewData["Venues"] = _context.Venues.ToList();
            return View(bookings);
        }
    }
}
