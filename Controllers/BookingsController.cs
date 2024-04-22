using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBC_Travel_Group_50.Models;

namespace GBC_Travel_Group_50.Controllers
{
    public class BookingsController : Controller
    {
        private readonly TravelBookingContext _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(TravelBookingContext context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewBag.Flights = new SelectList(_context.Flights.ToList(), "FlightID", "Airline");
            ViewBag.Hotels = new SelectList(_context.Hotels.ToList(), "HotelID", "HotelName");
            ViewBag.CarRentals = new SelectList(_context.CarRentals.ToList(), "RentalID", "RentalCompany");
            return View();
        }

        // POST: Bookings/Create]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,ServiceType,SelectedServiceID,StartDate,EndDate,TotalPrice,NumberOfPassengers")] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        // Log these messages or put a breakpoint here to inspect
                    }
                }
                PrepareCreateViewData();
                return View(booking);

            }
            if (ModelState.IsValid)
            {
                if (booking.ServiceType == ServiceType.Flight)
                {
                    var flight = await _context.Flights.FindAsync(booking.SelectedServiceID);
                    if (flight != null)
                    {
                        // Calculate total passengers already booked for this flight
                        int totalPassengersAlreadyBooked = _context.Bookings
                            .Where(b => b.SelectedServiceID == booking.SelectedServiceID && b.ServiceType == ServiceType.Flight)
                            .Sum(b => b.NumberOfPassengers);

                        // Check if adding the new booking exceeds the flight's capacity
                        if (totalPassengersAlreadyBooked + booking.NumberOfPassengers <= flight.Capacity)
                        {
                            // Proceed with adding the booking
                            _context.Add(booking);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Not enough capacity
                            ModelState.AddModelError("", "There are not enough seats available on this flight.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The selected flight could not be found.");
                    }
                }
                else
                {
                    // For non-flight services, just add the booking
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // If we get here, something failed; redisplay form
            PrepareCreateViewData(); 
            return View(booking);
        }


        private void PrepareCreateViewData()
        {
            // Populate any ViewBag properties required for the Create view here
            ViewBag.Flights = new SelectList(_context.Flights.ToList(), "FlightID", "Airline");
            ViewBag.Hotels = new SelectList(_context.Hotels.ToList(), "HotelID", "HotelName");
            ViewBag.CarRentals = new SelectList(_context.CarRentals.ToList(), "RentalID", "RentalCompany");
        }



        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,ServiceType,SelectedServiceID,StartDate,EndDate,TotalPrice, NumberOfPassengers")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }


        public async Task<JsonResult> GetServicesByType(ServiceType serviceType)
        {
            object services = null;

            switch (serviceType)
            {
                case ServiceType.Flight:
                    services = await _context.Flights.Select(f => new { id = f.FlightID, name = f.Airline }).ToListAsync();
                    break;
                case ServiceType.Hotel:
                    services = await _context.Hotels.Select(h => new { id = h.HotelID, name = h.HotelName }).ToListAsync();
                    break;
                case ServiceType.CarRental:
                    services = await _context.CarRentals.Select(c => new { id = c.RentalID, name = c.RentalCompany }).ToListAsync();
                    break;
            }

            return Json(services);
        }

        public async Task<IActionResult> CalculateTotalPrice(int serviceId, ServiceType serviceType, DateTime startDate, DateTime endDate, int NumberOfPassengers = 1)
        {
            try
            {
                float totalPrice = 0; 
                _logger.LogInformation($"Starting total price calculation for ServiceID: {serviceId}, ServiceType: {serviceType}, StartDate: {startDate}, EndDate: {endDate}");

                switch (serviceType)
                {
                    case ServiceType.Flight:
                        var flight = await _context.Flights.FindAsync(serviceId);
                        if (flight != null)
                        {
                            // For flights, calculate total price based on the number of passengers
                            totalPrice = flight.Price * NumberOfPassengers;
                            _logger.LogInformation($"Flight found: {flight.FlightID}. Price per passenger: {flight.Price}, Total price: {totalPrice}");
                        }
                        else
                        {
                            _logger.LogWarning($"Flight not found: {serviceId}");
                        }
                        break;

                    case ServiceType.Hotel:
                        var hotel = await _context.Hotels.FindAsync(serviceId);
                        if (hotel != null)
                        {
                            int days = (endDate - startDate).Days;
                            totalPrice = days * hotel.PricePerNight;
                            _logger.LogInformation($"Hotel found: {hotel.HotelID}. Price per night: {hotel.PricePerNight}, Total price: {totalPrice}");
                        }
                        else
                        {
                            _logger.LogWarning($"Hotel not found: {serviceId}");
                        }
                        break;

                    case ServiceType.CarRental:
                        var carRental = await _context.CarRentals.FindAsync(serviceId);
                        if (carRental != null)
                        {
                            int days = (endDate - startDate).Days;
                            totalPrice = days * carRental.PricePerDay;
                            _logger.LogInformation($"Car Rental found: {carRental.RentalID}. Price per day: {carRental.PricePerDay}, Total price: {totalPrice}");
                        }
                        else
                        {
                            _logger.LogWarning($"Car Rental not found: {serviceId}");
                        }
                        break;
                }

                _logger.LogInformation($"Total price calculated: {totalPrice}");
                return Json(new { totalPrice });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total price");
                return Json(new { error = "An error occurred while calculating the total price." });
            }
        }


    }
}
