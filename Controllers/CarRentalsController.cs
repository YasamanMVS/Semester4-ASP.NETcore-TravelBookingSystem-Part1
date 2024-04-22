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
    public class CarRentalsController : Controller
    {
        private readonly TravelBookingContext _context;

        public CarRentalsController(TravelBookingContext context)
        {
            _context = context;
        }

        //Search Functnality
        // GET: CarRentals
        public async Task<IActionResult> Index(string searchCompany, string searchModel, string searchLocation)
        {
            var carRentals = from cr in _context.CarRentals
                             select cr;

            if (!String.IsNullOrEmpty(searchCompany))
            {
                carRentals = carRentals.Where(cr => cr.RentalCompany.Contains(searchCompany));
            }

            if (!String.IsNullOrEmpty(searchModel))
            {
                carRentals = carRentals.Where(cr => cr.RentalModel.Contains(searchModel));
            }

            if (!String.IsNullOrEmpty(searchLocation))
            {
                carRentals = carRentals.Where(cr => cr.Location.Contains(searchLocation));
            }

            return View(await carRentals.ToListAsync());
        }

        // GET: CarRentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carRental = await _context.CarRentals
                .FirstOrDefaultAsync(m => m.RentalID == id);
            if (carRental == null)
            {
                return NotFound();
            }

            return View(carRental);
        }

        // GET: CarRentals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarRentals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalID,RentalCompany,RentalModel,PricePerDay,Location,Availability")] CarRental carRental)
        {

            if (ModelState.IsValid)
            {
                _context.Add(carRental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carRental);
        }

        // GET: CarRentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carRental = await _context.CarRentals.FindAsync(id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        // POST: CarRentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalID,RentalCompany,RentalModel,PricePerDay,Location,Availability")] CarRental carRental)
        {
            if (id != carRental.RentalID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carRental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarRentalExists(carRental.RentalID))
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
            return View(carRental);
        }

        // GET: CarRentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carRental = await _context.CarRentals
                .FirstOrDefaultAsync(m => m.RentalID == id);
            if (carRental == null)
            {
                return NotFound();
            }

            return View(carRental);
        }

        // POST: CarRentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carRental = await _context.CarRentals.FindAsync(id);
            if (carRental != null)
            {
                _context.CarRentals.Remove(carRental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarRentalExists(int id)
        {
            return _context.CarRentals.Any(e => e.RentalID == id);
        }
    }
}
