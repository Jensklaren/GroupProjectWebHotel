﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupProjectWebHotel.Data;
using GroupProjectWebHotel.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GroupProjectWebHotel.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index(string sortOrder)
        {
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            var book = (IQueryable<Booking>)_context.Booking.Include(b => b.TheCustomer).Where(w => w.CustomerEmail == _email).Include(b => b.TheRoom);

            if (String.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "CheckIn_asc";
            }

            switch (sortOrder)
            {
                case "CheckIn_asc":
                    book = book.OrderBy(p => p.CheckIn);
                    break;
                case "pizzaName_desc":
                    book = book.OrderByDescending(p => p.CheckIn);
                    break;
                case "pizzaCount_asc":
                    book = book.OrderBy(p => p.Cost);
                    break;
                case "pizzaCount_desc":
                    book = book.OrderByDescending(p => p.Cost);
                    break;
            }

            ViewData["NextCheckInOrder"] = sortOrder != "CheckIn_asc" ? "CheckIn_asc" : "CheckIn_desc";
            ViewData["NextCostOrder"] = sortOrder != "Cost_asc" ? "Cost_asc" : "Cost_desc";
            return View(await book.AsNoTracking().ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
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
                    if (!BookingExists(booking.ID))
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
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
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
            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CalcPurchaseStats(CalculateStats calculateStats)
        {
            var customerCountGroups = _context.Customer.GroupBy(m => m.Postcode);
            var customerStatsPostcode = customerCountGroups.Select(g => new CalculateStats { CustomersPostcode = g.Key, PostcodeCount = g.Count() });

            var customerCountGroups2 = _context.Booking.GroupBy(m => m.RoomID);
            var customerStatsRooms = customerCountGroups2.Select(g => new CalculateStats { CustomersRoom = g.Key, RoomIDCount = g.Count() });

            ViewBag.CustomerPostcodeStats = await customerStatsPostcode.ToListAsync();
            ViewBag.CustomerRoomStats = await customerStatsRooms.ToListAsync();

            return View(await customerStatsPostcode.ToListAsync());
        
        }
    }

}
