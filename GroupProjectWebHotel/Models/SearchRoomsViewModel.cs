﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace GroupProjectWebHotel.Models
{
    public class SearchRoomsViewModel
    {
        [Required]
        public string BedCount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }
    }
}