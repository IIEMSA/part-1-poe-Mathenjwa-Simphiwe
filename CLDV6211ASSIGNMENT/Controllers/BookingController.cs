using CLDV6211ASSIGNMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CLDV6211ASSIGNMENT.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDBcontext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(ApplicationDBcontext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                // Start with base query
                var bookingsQuery = _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .Where(b => b.Event != null && b.Venue != null); // Filter nulls first

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    bookingsQuery = bookingsQuery.Where(b =>
                        b.Event.EventName.ToLower().Contains(searchString) ||
                        b.Venue.VenueName.ToLower().Contains(searchString)
                    );
                }

                // Project to ViewModel
                var bookings = await bookingsQuery
                    .Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        EventName = b.Event.EventName,
                        VenueName = b.Venue.VenueName,
                        BookingDate = b.BookingDate
                    })
                    .ToListAsync();

                return View(bookings);
            }
            catch (Exception ex)
            {
                // Make sure you have ILogger<BookingController> injected in constructor
                _logger.LogError(ex, "Error loading bookings");
                TempData["ErrorMessage"] = "An error occurred while loading bookings.";
                return View(new List<BookingViewModel>()); // Return empty list on error
            }
        }
        // GET: Bookings/Create
        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewBag.EventId = new SelectList(_context.Eventss, "Id", "EventName");
            ViewBag.VenueId = new SelectList(_context.Venues, "Id", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId,BookingDate")] Bookings booking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Additional validation
                    if (booking.BookingDate < DateTime.Now)
                    {
                        ModelState.AddModelError("BookingDate", "Booking date cannot be in the past");
                    }
                    else if (!_context.Eventss.Any(e => e.Id == booking.EventId))
                    {
                        ModelState.AddModelError("EventId", "Selected event does not exist");
                    }
                    else if (!_context.Venues.Any(v => v.Id == booking.VenueId))
                    {
                        ModelState.AddModelError("VenueId", "Selected venue does not exist");
                    }
                    else
                    {
                        // Check for booking conflicts
                        var conflict = await _context.Bookings
                            .AnyAsync(b => b.VenueId == booking.VenueId &&
                                          b.BookingDate.Date == booking.BookingDate.Date);

                        if (conflict)
                        {
                            ModelState.AddModelError("", "The venue is already booked for this date");
                        }
                        else
                        {
                            _context.Add(booking);
                            await _context.SaveChangesAsync();
                            TempData["Success"] = "Booking created successfully";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", "An error occurred while creating the booking");
            }

            // Repopulate dropdowns if there's an error
            ViewBag.EventId = new SelectList(_context.Eventss, "Id", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "Id", "VenueName", booking.VenueId);
            return View(booking);
        }
        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.EventId = new SelectList(_context.Eventss.Where(e => e != null), "Id", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues.Where(v => v != null), "Id", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,VenueId,BookingDate")] Bookings booking)
        {
            if (id != booking.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                        return NotFound();

                    throw;
                }
            }

            ViewBag.EventId = new SelectList(_context.Eventss, "Id", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "Id", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking");
                TempData["Error"] = "An error occurred while deleting the booking.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)   // ✅ required to prevent null access in view
                .Include(b => b.Venue)   // ✅ same here
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }


        private bool BookingExists(int id)
            {
                return _context.Bookings.Any(e => e.Id == id);
            }
        }
    }
