using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GroupProjectWebHotel.Models
{
    public class Customer
    {
        [Key, Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z - ']{2,20}$")]
        [Display(Name = "Customer Surname")]
        public string Surname { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z - ']{2,20}$")]
        [Display(Name = "Customer Given Name")]
        public string GivenName { get; set; }

        [NotMapped]
        public string FullName => $"{GivenName} {Surname}";

        [Required]
        [RegularExpression(@"[0-9]{4}$")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        public ICollection<Booking> TheBookings { get; set; }
    }
}