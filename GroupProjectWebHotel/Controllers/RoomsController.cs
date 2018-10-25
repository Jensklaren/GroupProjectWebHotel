using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupProjectWebHotel.Data;
using GroupProjectWebHotel.Models;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;

namespace GroupProjectWebHotel.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Room.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Level,BedCount,Price")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Level,BedCount,Price")] Room room)
        {
            if (id != room.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.ID))
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
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.SingleOrDefaultAsync(m => m.ID == id);
            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.ID == id);
        }

        [AllowAnonymous]
        public IActionResult SearchRooms()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SearchRooms(SearchRoomsViewModel searchRooms)
        {
          
                var Bedcount = new SqliteParameter("BedCount", searchRooms.BedCount);
                var CheckIn = new SqliteParameter("CheckIn", searchRooms.CheckIn);
                var CheckOut = new SqliteParameter("CheckOut", searchRooms.CheckOut);

                var CheckRooms = _context.Room.FromSql("select * from [Room] "
                   + " where [Room].BedCount = @BedCount ", Bedcount)
                  // + "SELECT * FROM [Booking] WHERE[Booking].CheckIn = @CheckIn not in", Bedcount, CheckIn)
                  //  + "WHERE[Booking].CheckIn = @CheckIn Not In AND Where [Booking].CheckOut = @CheckOut)", Bedcount, CheckIn, CheckOut)
                  .Select(ro => new Room { ID = ro.ID, Level = ro.Level, BedCount = ro.BedCount, Price = ro.Price });

          /*  var diffMovies = _context.Movie.FromSql("select * from [Movie] inner join [Order] on [Movie].ID = [Order].MovieID "
                               + "where [Order].MovieGoerEmail = @personA and [Movie].ID not in "
                               + "(select [Movie].ID from [Movie] inner join [Order] on [Movie].ID = [Order].MovieID "
                               + "where[Order].MovieGoerEmail = @personB)", emailA, emailB)
                               */

                ViewBag.Rooms = await CheckRooms.ToListAsync();
     
            return View(searchRooms);
        }
    }
}
