//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExtractBostonCitizensConnect
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCitizensConnectDump
    {
        public int ID { get; set; }
        public System.DateTime DateExtracted { get; set; }
        public Nullable<decimal> TotalCount { get; set; }
        public Nullable<decimal> OpenCount { get; set; }
        public Nullable<decimal> ClosedCount { get; set; }
        public string ServiceName { get; set; }
        public Nullable<decimal> ServiceTotalCount { get; set; }
        public Nullable<decimal> ServiceOpenCount { get; set; }
        public Nullable<decimal> ServiceCloseCount { get; set; }
        public string ReportURL { get; set; }
        public string Status { get; set; }
        public string ReportAddress { get; set; }
        public string ReportCoorXY { get; set; }
        public string ReportCoorLatLon { get; set; }
        public string OpenedTime { get; set; }
        public string ClosedTime { get; set; }
        public string ClosedMsg { get; set; }
        public string SubmittedTime { get; set; }
        public string SubmittedVia { get; set; }
        public string Comments { get; set; }
        public string XCoordinates { get; set; }
        public string YCoordinates { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ReportText { get; set; }
    }
}
