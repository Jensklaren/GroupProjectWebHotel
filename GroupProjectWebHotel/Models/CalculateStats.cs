using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GroupProjectWebHotel.Models
{
    public class CalculateStats
    {
            [Display(Name = "Postcode")]
            public string CustomersPostcode { get; set; }
            [Display(Name = "Number Of Customers")]
            public int PostcodeCount { get; set; }

            [Display(Name = "Room ID")]
            public int CustomersRoom { get; set; }
            [Display(Name = "Number of Bookings")]
            public int RoomIDCount { get; set; }
    }
}
