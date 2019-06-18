using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore22Mvc.Models.CustomModels
{
    public class Admin_Settings
    {
        [Key]
        public int Id { get; set; }
        public string AdminEmailId { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public string MicrosoftClientId { get; set; }
        public string MicrosoftClientSecret { get; set; }
        public string SendGridKey { get; set; }

    }
}
