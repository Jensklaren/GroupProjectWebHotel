using System;
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
using Microsoft.Data.Sqlite;

namespace GroupProjectWebHotel.Controllers
{
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
        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookRoom bookRoom)
        {
            string _email = User.FindFirst(ClaimTypes.Name).Value;

            //Calculate total days
            var totalDays = (decimal)(bookRoom.CheckOut - bookRoom.CheckIn).TotalDays;

            //Assigning values
            var roomID = new SqliteParameter("RoomID", bookRoom.RoomID);
            var email = new SqliteParameter("CustomerEmail", _email);
            var checkIn = new SqliteParameter("CheckIn", bookRoom.CheckIn);
            var checkOut = new SqliteParameter("CheckOut", bookRoom.CheckOut);

            //Select the Room price
            var selected = _context.Room.FromSql("SELECT * FROM Room WHERE Room.ID = @RoomID", roomID)
                .Select(ro => new Room { ID = ro.ID, Price = ro.Price });
            List<Room> selectedRoom = await selected.ToListAsync();
            var roomPrice = selectedRoom.ElementAt(0).Price;

            //Total days times room price 
            var calcCost = totalDays * roomPrice;

            //Assign cost to the calculated cost
            var cost = new SqliteParameter("Cost", calcCost);

            //if (ModelState.IsValid)
            //{

            ViewBag.RowsAffected = await _context.Database.ExecuteSqlCommandAsync("INSERT INTO Booking (RoomID, CustomerEmail, CheckIn, CheckOut, Cost) " +
                    "VALUES (@RoomID, @CustomerEmail, @CheckIn, @CheckOut, @Cost)", roomID, email, checkIn, checkOut, cost);

            ViewBag.TotalCost = calcCost;
            //}

            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", bookRoom.RoomID);

            return View(bookRoom);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageCreate()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            ViewData["RoomCost"] = new SelectList(_context.Room, "Cost", "Cost");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageCreate([Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageIndex));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID", booking.RoomID);
            return View(booking);
        }
        //
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageIndex()
        {
            var applicationDbContext = _context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
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
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID", booking.RoomID);
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
                return RedirectToAction(nameof(ManageIndex));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID", booking.RoomID);

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
            return RedirectToAction(nameof(ManageIndex));
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
