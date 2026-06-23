using System;
using System.Collections.Generic;
using System.Text;

namespace VeteransBannerCRM.Models
{
    public class DashboardCount
    {
        public string Location { get; set; } = "";
        public string Season { get; set; } = "";
        public int OrderCount { get; set; }
    }
}