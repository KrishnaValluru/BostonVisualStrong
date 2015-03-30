using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExtractBostonCitizensConnect
{

    /// <summary>
    /// Extract data from "https://mayors24.cityofboston.gov/";
    /// </summary>
    public class ExtractData
    {
        //database
        int _intDateID = 0;  // this is the job run date
        string masterURL = "https://mayors24.cityofboston.gov/";

        public ExtractData()
        {
            GetDateID();
        }

//TFS.........checkin.......
       
    }
}
