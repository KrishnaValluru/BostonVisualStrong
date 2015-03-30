using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtractBostonCitizensConnect.Models
{
    //https://mayors24.cityofboston.gov:4443/?page=1
    /// <summary>
    /// this is to extract services from https://mayors24.cityofboston.gov:4443/?page=1
    /// 
    /// </summary>
    public class Services
    {
        public string ServiceName { get; set; }

        public int TotalPosts { get; set; }

        public int OpenPosts { get; set; }

        public int ClosedPosts { get; set; }

        public CitizenPosts[] CitizenPosts { get; set; }
    }

   
}
