using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProjectWebHotel.Models
{
    public class Booking
    {
        public int ID { get; set; }
        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        public string CustomerEmail { get; set; }

        [DataType(DataType.Date), Display(Name = "Checking in Date")]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date), Display(Name = "Checking out Date")]
        public DateTime CheckOut { get; set; }
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }

    }
}
