using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProjectWebHotel.Models
{
    public class Room
    {
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"[G123]{1}")]
        public string Level { get; set; }

        [RegularExpression(@"[123]{1}")]
        public int BedCount { get; set; }

        [Range(50, 300)]
        public decimal Price { get; set; }

        public ICollection<Booking> TheBookings { get; set; }


    }
}
