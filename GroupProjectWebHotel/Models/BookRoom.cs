using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProjectWebHotel.Models
{
    public class BookRoom
    {
        [Key, Required]
        [Range(1, 16)]
        public int RoomID { get; set; }
        public string CustomerEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }
    }
}
