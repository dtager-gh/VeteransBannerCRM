using System;
using System.Collections.Generic;
using System.Text;

namespace VeteransBannerCRM.Models
{
    public class BannerOrder
    {
        public int OrderID { get; set; }

        public string Season { get; set; } = "";
        public string Location { get; set; } = "";

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string Branch { get; set; } = "";
        public string WarConflict { get; set; } = "";
        public string Medals { get; set; } = "";

        public string BannerType { get; set; } = "";
        public string PhotoLayout { get; set; } = "";

        public string PairedWith { get; set; } = "";

        public string Contact { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public string LivingDeceased { get; set; } = "";
        public bool KIA { get; set; }

        public string Misc { get; set; } = "";
        public string NameOnBanner { get; set; } = "";

        public string Notes { get; set; } = "";
    }
}
