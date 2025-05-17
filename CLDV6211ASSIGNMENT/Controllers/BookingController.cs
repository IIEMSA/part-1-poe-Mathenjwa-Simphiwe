using CLDV6211ASSIGNMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        
            // GET: Bookings
            public async Task<IActionResult> Index(string searchString)
            {
                var bookingsQuery = _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    bookingsQuery = bookingsQuery.Where(b =>
                        (b.Event != null && b.Event.EventName.ToLower().Contains(searchString)) ||
                        (b.Venue != null && b.Venue.VenueName.ToLower().Contains(searchString))
                    );
                }

                var bookings = await bookingsQuery.ToListAsync();
                return View(bookings);
            }

            // GET: Bookings/Create
            public IActionResult Create()
            {
                ViewBag.EventID = new SelectList(_context.Eventss, "EventID", "EventName");
                ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "VenueName");
                return View();
            }

            // POST: Bookings/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Bookings booking)
            {
                ViewBag.EventID = new SelectList(_context.Eventss, "EventID", "EventName", booking.Id);
                ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueId);

                if (ModelState.IsValid)
                {
                    var conflict = await _context.Bookings
                        .AnyAsync(b => b.VenueId == booking.VenueId && b.BookingDate.Date == booking.BookingDate.Date);

                    if (conflict)
                    {
                        ModelState.AddModelError("", "The selected venue is already booked for the specified date.");
                        return View(booking);
                    }

                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(booking);
            }

            // GET: Bookings/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                    return NotFound();

                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                    return NotFound();

                ViewBag.EventID = new SelectList(_context.Eventss, "EventID", "EventName", booking.EventId);
                ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueId);

                return View(booking);
            }

            // POST: Bookings/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, Bookings booking)
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
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException?.Message.Contains("FK_Bookings_VenueID") == true)
                        {
                            ModelState.AddModelError("", "Invalid Venue. Ensure the selected venue exists.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "An error occurred while updating the booking.");
                        }
                    }
                }

                ViewBag.EventID = new SelectList(_context.Eventss, "EventID", "EventName", booking.EventId);
                ViewBag.VenueID = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueId);

                return View(booking);
            }

            // GET: Delete Bookings
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                    return NotFound();

                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (booking == null)
                    return NotFound();

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
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking deleted successfully.";
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("FK_Bookings_EventID") == true ||
                        ex.InnerException?.Message.Contains("FK_Bookings_VenueID") == true)
                    {
                        TempData["ErrorMessage"] = "You cannot delete this booking as it is associated with other records.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while trying to delete the booking.";
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // GET: Bookings/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                    return NotFound();

                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
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
