using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommApplication.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "User Id cannot be empty.")]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password cannot be empty.")]
        [StringLength(25)]
        [DisplayName("Password")]
        public string Pwd { get; set; }

        [Required(ErrorMessage = "Please select your Role.")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Mobile number should not be empty.")]
        public long? MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        public string Email { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Please select your Location.")]
        public string Location { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}