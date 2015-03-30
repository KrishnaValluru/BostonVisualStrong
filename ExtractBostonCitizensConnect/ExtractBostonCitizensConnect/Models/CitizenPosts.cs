using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtractBostonCitizensConnect.Models
{
    public class CitizenPosts
    {

        //to store ID 101001334277
        public long ID { get; set; }

        //to store Description of Space Saver: barrels cones and chairs. they are all over the area
        public string Description { get; set; }

        //to store address: 49 Mount Ida Rd, Dorchester
        public string Address { get; set; }

        //to store coordinates x,y: 773907.1137200015, 2936583.324730004
        public string CoordinatesxNy { get; set; }

        //to store coordinates lat,lng: 42.30531000016374, -71.0647399995934
        public string CoordinatesLatNLon { get; set; } 
    }
}
