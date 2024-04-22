using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBC_Travel_Group_50.Models;

namespace GBC_Travel_Group_50.Controllers
{
    public class FlightsController : Controller
    {
        private readonly TravelBookingContext _context;

        public FlightsController(TravelBookingContext context)
        {
            _context = context;
        }


        //search functunality
        // GET: Flights
        public async Task<IActionResult> Index(string searchDestination, string searchOrigin, DateTime? departureDate, DateTime? arrivalDate, int? searchCapacity)
        {
            var flights = from m in _context.Flights
                          select m;

            if (!String.IsNullOrEmpty(searchOrigin))
            {
                flights = flights.Where(s => s.Origin.Contains(searchOrigin));
            }
            if (!String.IsNullOrEmpty(searchDestination))
            {
                flights = flights.Where(s => s.Destination.Contains(searchDestination));
            }
            if (departureDate.HasValue)
            {
                var departureStartOfDay = departureDate.Value.Date;
                var departureEndOfDay = departureStartOfDay.AddDays(1).AddTicks(-1);
                flights = flights.Where(f => f.Departure >= departureStartOfDay && f.Departure <= departureEndOfDay);
            }
            if (arrivalDate.HasValue)
            {
                var arrivalStartOfDay = arrivalDate.Value.Date;
                var arrivalEndOfDay = arrivalStartOfDay.AddDays(1).AddTicks(-1);
                flights = flights.Where(f => f.Arrival >= arrivalStartOfDay && f.Arrival <= arrivalEndOfDay);
            }
            if (searchCapacity.HasValue)
            {
                // Filter flights based on the specified capacity
                flights = flights.Where(f => f.Capacity >= searchCapacity.Value);
            }

            return View(await flights.ToListAsync());
        }

        // old controller: lists all flights regardless of anyfilters
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Flights.ToListAsync());
        //}

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .FirstOrDefaultAsync(m => m.FlightID == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightID,Airline,Departure,Arrival,Origin,Destination,Price,Capacity")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightID,Airline,Departure,Arrival,Origin,Destination,Price,Capacity")] Flight flight)
        {
            if (id != flight.FlightID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.FlightID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .FirstOrDefaultAsync(m => m.FlightID == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.FlightID == id);
        }
    }
}
