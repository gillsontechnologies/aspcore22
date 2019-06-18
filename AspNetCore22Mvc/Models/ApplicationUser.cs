using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore22Mvc.Models
{
    //created model which use in class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "First Name")]
        public string Name { get; set; }

        [PersonalData]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [PersonalData]
        public string Organisation { get; set; }

        [PersonalData]
        [Display(Name = "Position/Title")]
        public string Position { get; set; }
        [PersonalData]
        public bool isApproved { get; set; }
    }
}

