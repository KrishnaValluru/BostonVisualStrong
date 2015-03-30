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


        /// <summary>
        /// Start Process to load masterURL page and to loop through Services
        /// </summary>
        public void StartProcess()
        {
            //Thanks to HTML Agility Pack to extract HTML elements...
            HtmlAgilityPack.HtmlDocument docHTML = new HtmlAgilityPack.HtmlDocument();

            try
            {

                HtmlAgilityPack.HtmlWeb docHFile = new HtmlWeb();

                //set cookie
                docHFile.PreRequest += request =>
                {
                    request.CookieContainer = new System.Net.CookieContainer();
                    return true;
                };

                docHTML = docHFile.Load(masterURL); // load url

                Debug.Assert(docHTML.ParseErrors.Count() == 0, "DocHTML is null");

                //if no Parse errors
                if (docHTML.ParseErrors.Count() == 0)
                {
                    //Get Total Count
                    // TotalCount.InnerText = "View Reports - 491,059 found"
                    // <h2>View Reports - 491,084 found</h2>
                    int TotalCount = GetNum(docHTML.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/div[1]/h2").InnerText);


                    //Get Open Count
                    // //*[@id="facets"]/ul[1]/li[1]/span
                    //<a href="/?status=open">Open</a>
                    //<span class="muted">(34,238)</span>
                    int OpenCount = GetNum(docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[1]/li[1]/span").InnerText);

                    //Get Closed count
                    // //*[@id="facets"]/ul[1]/li[2]/span
                    //<a href="/?status=closed">Closed</a>
                    //<span class="muted">(456,846)</span>
                    int ClosedCount = GetNum(docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[1]/li[2]/span").InnerText);

                    //Get Services loop
                    // //*[@id="facets"]/ul[2]

                    //For each sevrice loop
                    //
                    HtmlNode docUL = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[2]");

                    foreach (HtmlNode anchorTag in docUL.SelectNodes(".//li"))
                    {
                        // //*[@id="facets"]/ul[2]/li[1]/a
                        //<a href="/?service_id=50af779a006aed4a5da85b14">Snow/Ice Control</a>
                        //string ServicesName = anchorTag.SelectSingleNode("//*[@id=\"facets\"]/ul[2]/li[1]/a").InnerText;
                        string ServicesName = anchorTag.SelectSingleNode(".//a").InnerText;

                        // //*[@id=\"facets\"]/ul[2]/li[1]/span
                        //<span class="muted">(68,240)</span>
                        int ServiceCount = GetNum(anchorTag.SelectSingleNode(".//span").InnerText);

                        string ServiceHref = anchorTag.SelectSingleNode(".//a").Attributes["href"].Value;



                        ProcessService(TotalCount, OpenCount, ClosedCount, ServicesName, ServiceCount, ServiceHref);
                    }
                }


            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //TODO
            }
        }

        /// <summary>
        /// for each service get the report id
        /// </summary>
        /// <param name="TotalCount"></param>  SHOULD Have created models but no time.....visit later to REFRACTOR.....
        /// <param name="OpenCount"></param>
        /// <param name="ClosedCount"></param>
        /// <param name="ServicesName"></param>
        /// <param name="ServiceCount"></param>
        /// <param name="ServiceHref"></param>
        private void ProcessService(int TotalCount, int OpenCount, int ClosedCount, string ServicesName, int ServiceCount, string ServiceHref)
        {

            HtmlAgilityPack.HtmlDocument docHTML = new HtmlAgilityPack.HtmlDocument();

            try
            {

                HtmlAgilityPack.HtmlWeb docHFile = new HtmlWeb();

                for (int i = 1; i <= 20; i++)
                {
                    string baseServiceURL = "https://mayors24.cityofboston.gov";
                    // to form https://mayors24.cityofboston.gov/?service_id=50af779a006aed4a5da85b14


                    baseServiceURL = baseServiceURL + "/?page=" + i.ToString() + ServiceHref.Replace("/?", "&");

                    //set cookie
                    docHFile.PreRequest += request =>
                    {
                        request.CookieContainer = new System.Net.CookieContainer();
                        return true;
                    };

                    docHTML = docHFile.Load(baseServiceURL); // load url

                    Debug.Assert(docHTML.ParseErrors.Count() == 0, "DocHTML is null");

                    //if no Parse errors
                    if (docHTML.ParseErrors.Count() == 0)
                    {

                        int ServiceOpenCount=0;
                        int ServiceCloseCount=0;

                        //GetServiceOpenCOunt
                        if (docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[1]/li[1]/span") != null)
                            ServiceOpenCount = GetNum( docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[1]/li[1]/span").InnerText);

                        //GetServiceCloseCount
                        // //*[@id="facets"]/ul[1]/li[2]/span
                        if (docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[1]/li[2]/span") != null)
                            ServiceCloseCount = GetNum(docHTML.DocumentNode.SelectSingleNode("//*[@id=\"facets\"]/ul[1]/li[2]/span").InnerText);

                        HtmlNode docTR = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"reports\"]/tbody"); //docHTML.DocumentNode.SelectSingleNode("//ul[@class=\"directory\"]");

                        foreach (HtmlNode trTag in docTR.SelectNodes(".//tr"))
                        {
                            ////*[@id="reports"]/tbody/tr[1]/td[1]/span[2]
                            string docSpanID = trTag.SelectSingleNode(".//td[1]/span[2]").InnerText;

                            string[] words = docSpanID.Split('#');

                            string reportID = words[1].Trim();

                            GetReport(TotalCount, OpenCount, ClosedCount, ServicesName, ServiceCount, ServiceOpenCount, ServiceCloseCount, ServiceHref, reportID);

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //TODO
            }
        }

        /// <summary>
        /// Get Report using the reportID and extract 1) Status of the case 2) report TEXT 3) XY coor. 4) Lat and Lon.
        /// </summary>
        /// <param name="TotalCount"></param>
        /// <param name="OpenCount"></param>
        /// <param name="ClosedCount"></param>
        /// <param name="ServicesName"></param>
        /// <param name="ServiceCount"></param>
        /// <param name="ServiceOpenCount"></param>
        /// <param name="ServiceCloseCount"></param>
        /// <param name="ServiceHref"></param>
        /// <param name="reportID"></param>
        private void GetReport(int TotalCount, int OpenCount, int ClosedCount, string ServicesName, int ServiceCount, int ServiceOpenCount, int ServiceCloseCount, string ServiceHref, string reportID)
        {
            string reportBaseURL = "https://mayors24.cityofboston.gov/reports/";
            HtmlAgilityPack.HtmlDocument docHTML = new HtmlAgilityPack.HtmlDocument();
            docHTML.OptionFixNestedTags = true;
            HtmlNode.ElementsFlags.Remove("p"); // remove the Empty and Closed flags

            try
            {
                HtmlAgilityPack.HtmlWeb docHFile = new HtmlWeb();
                reportBaseURL = reportBaseURL + reportID;

                //set cookie
                docHFile.PreRequest += request =>
                {
                    request.CookieContainer = new System.Net.CookieContainer();
                    return true;
                };



                docHTML = docHFile.Load(reportBaseURL); // load url

                //Debug.Assert(docHTML.ParseErrors.Count() > 2, "DocHTML is null");

                ////if no Parse errors
                //if (docHTML.ParseErrors.Count() == 0)
                //{
                //Get ID
                // about 3 hours ago #101001334904
                // //*[@id="report-source"]/strong

                //Get OPENED / CLOSED
                // //*[@id="report-source"]/span
                string status = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"report-source\"]/span").InnerText;

                //Get Text  //*[@id="content"]/div[2]/blockquote/p/text()
                // //*[@id="content"]/div[2]/blockquote/p/text()
                // //*[@id="content"]/div[2]/blockquote/p
                ////*[@id="content"]/div[2]

                string[] splitWords;

                string reportText = reportText = docHTML.DocumentNode.SelectSingleNode("//blockquote/p").InnerText;

                //Get Address
                // //*[@id="location-tab"]/p[1]/text()
                string reportAddress = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"location-tab\"]/p[1]").InnerText;
                if (reportAddress.Length > 0)
                {
                    reportAddress = reportAddress.Replace("address:", "").Replace("&amp;", "&").Trim();

                }

                //Get CoordinatesXY
                // //*[@id="location-tab"]/p[2]/text()
                string reportCoordinatesXY = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"location-tab\"]/p[2]").InnerText;
                string xCoor = "";
                string yCoor = "";
                if (reportCoordinatesXY.Length > 0)
                {
                    reportCoordinatesXY = reportCoordinatesXY.Replace("coordinates x,y:", "").Trim();
                    splitWords = reportCoordinatesXY.Split(',');
                    xCoor = splitWords[0];
                    yCoor = splitWords[1];
                }

                //Get Coordinates LATLON
                // //*[@id="location-tab"]/p[3]/a

                string reportCoordinatesLATLON = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"location-tab\"]/p[3]").InnerText;
                string latitude = "";
                string longitude = "";
                if (reportCoordinatesLATLON.Length > 0)
                {
                    reportCoordinatesLATLON = reportCoordinatesLATLON.Replace("coordinates lat,lng:", "").Trim();

                    splitWords = reportCoordinatesLATLON.Split(',');
                    latitude = splitWords[0];
                    longitude = splitWords[1];
                }



                //Get NOTES_OPened  IF OPEN
                // //*[@id="notes-tab"]/table/tbody/tr[3]/td[2] is Opened   THEN GET TIME from  //*[@id="notes-tab"]/table/tbody/tr[5]/td[1]
                // //*[@id="notes-tab"]/table/tbody/tr[3]/td[2]/text()  will have closed  THEN GET TIME From //*[@id="notes-tab"]/table/tbody/tr[3]/td[1]

                // //*[@id="notes-tab"]/table/tbody/tr
                // HtmlNode docTR = docHTML.DocumentNode.SelectSingleNode("//*[@id=\"notes-tab\"]/table/tr");

                //? docTR = docHTML.DocumentNode.SelectSingleNode("//table/tr[1]");  ****TODO

                string openedTime = "";
                string closedTime = "";
                string closedMsg = "";
                string submittedTime = "";
                string SubmittedVia = "";


                foreach (HtmlNode tdTag in docHTML.DocumentNode.SelectNodes("//table/tr"))
                {
                    var node = tdTag.SelectNodes(".//td");
                    if (node != null)
                    {
                        string tdElement = tdTag.InnerHtml.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("</td><td>", "|").Replace("<td>", "").Replace("</td>", "");

                        //if (status.ToUpper() == "OPENED")
                        //else if (status.ToUpper() == "CLOSED")

                        if (tdElement.IndexOf("|opened", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            try
                            {
                                splitWords = tdElement.Split('|');
                                openedTime = splitWords[0];
                            }
                            catch (Exception)
                            {
                                //TODO 
                            }

                        }
                        else if (tdElement.IndexOf("case closed", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            try
                            {
                                splitWords = tdElement.Split('|');
                                closedTime = splitWords[0];
                                closedMsg = splitWords[1];
                            }
                            catch (Exception)
                            {
                                //TODO
                            }

                        }
                        else if (tdElement.IndexOf("submitted", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            try
                            {
                                splitWords = tdElement.Split('|');
                                submittedTime = splitWords[0];
                                SubmittedVia = splitWords[1];
                                if (SubmittedVia.Length > 0)
                                    SubmittedVia = SubmittedVia.Remove(SubmittedVia.IndexOf("<img ")).Replace("Submitted via ", "").Trim();

                            }
                            catch (Exception)
                            {

                                //todo 
                            }

                        }

                        foreach (var tdItem in node)
                        {
                            string item = tdItem.InnerHtml;
                        }
                    }
                }


                //Get NOTES_Submitted

                //Debug.Print("Total Count = {0}; Open Count = {1}; Closed Count = {2}; Service Name = {3}; Service Count= {4}; baseServiceURL = {5}; Status = {6}; Report Text =|^{7}|^; ReportAddress = {8}; ReportCoordinatesXY={9}; ReportCoordinatesLATLON={10}; Opened Time={11}; Closed Time={12}; ClosedMsg= {13}; SubmitedTime={14}; SubmittedVia={15} ", TotalCount, OpenCount, ClosedCount, ServicesName, ServiceCount, reportBaseURL, status, reportText, reportAddress, reportCoordinatesXY, reportCoordinatesLATLON, openedTime, closedTime, closedMsg, submittedTime, SubmittedVia);
                Debug.Print("Total Count = {0}; Open Count = {1}; Closed Count = {2}; Service Name = {3}; Service Count= {4}; ServiceOpen= {5}; ServiceClose={6}; baseServiceURL = {7}; Status = {8}; Report Text =|^{9}|^; ReportAddress = {10}; ReportCoordinatesXY={11}; xCoordinates={12}; yCoordinates={13}; ReportCoordinatesLATLON={14}; latitude={15}; longitude={16}; Opened Time={17}; Closed Time={18}; ClosedMsg= {19}; SubmitedTime={20}; SubmittedVia={21} ", TotalCount, OpenCount, ClosedCount, ServicesName, ServiceCount, ServiceOpenCount,ServiceCloseCount,reportBaseURL, status, reportText, reportAddress, reportCoordinatesXY, xCoor, yCoor, reportCoordinatesLATLON, latitude, longitude, openedTime, closedTime, closedMsg, submittedTime, SubmittedVia);


                InsertData(TotalCount, OpenCount, ClosedCount, ServicesName, ServiceCount, ServiceOpenCount, ServiceCloseCount, reportBaseURL, status, reportText, reportAddress, reportCoordinatesXY, xCoor, yCoor, reportCoordinatesLATLON, latitude, longitude, openedTime, closedTime, closedMsg, submittedTime, SubmittedVia);

                //} close if {}

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //TODO
            }





        }


        /// <summary>
        /// Insert into database using Entity Object
        /// </summary>
        /// <param name="TotalCount"></param>
        /// <param name="OpenCount"></param>
        /// <param name="ClosedCount"></param>
        /// <param name="ServicesName"></param>
        /// <param name="ServiceCount"></param>
        /// <param name="ServiceOpenCount"></param>
        /// <param name="ServiceCloseCount"></param>
        /// <param name="reportBaseURL"></param>
        /// <param name="status"></param>
        /// <param name="reportText"></param>
        /// <param name="reportAddress"></param>
        /// <param name="reportCoordinatesXY"></param>
        /// <param name="xCoor"></param>
        /// <param name="yCoor"></param>
        /// <param name="reportCoordinatesLATLON"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="openedTime"></param>
        /// <param name="closedTime"></param>
        /// <param name="closedMsg"></param>
        /// <param name="submittedTime"></param>
        /// <param name="SubmittedVia"></param>
        private void InsertData(int TotalCount, int OpenCount, int ClosedCount, string ServicesName, int ServiceCount, int ServiceOpenCount, int ServiceCloseCount, string reportBaseURL, string status, string reportText, string reportAddress, string reportCoordinatesXY, string xCoor, string yCoor, string reportCoordinatesLATLON, string latitude, string longitude, string openedTime, string closedTime, string closedMsg, string submittedTime, string SubmittedVia)
        {
            try
            {
                CitizensConnectEntities entityObj = new CitizensConnectEntities();

                tblCitizensConnectDump tblObj = new tblCitizensConnectDump();

                tblObj.DateExtracted = DateTime.Today;
                tblObj.TotalCount = TotalCount;
                tblObj.OpenCount = OpenCount;
                tblObj.ClosedCount = ClosedCount;
                tblObj.ServiceName = ServicesName;
                tblObj.ServiceTotalCount = ServiceCount;
                tblObj.ServiceOpenCount = ServiceOpenCount;
                tblObj.ServiceCloseCount = ServiceCloseCount;
                tblObj.ReportURL = reportBaseURL;
                tblObj.Status = status;
                tblObj.ReportText = reportText;
                tblObj.ReportAddress = reportAddress;
                tblObj.ReportCoorXY = reportCoordinatesXY;
                tblObj.XCoordinates = xCoor;
                tblObj.YCoordinates = yCoor;
                tblObj.ReportCoorLatLon = reportCoordinatesLATLON;
                tblObj.Latitude = latitude;
                tblObj.Longitude = longitude;
                tblObj.OpenedTime = openedTime;
                tblObj.ClosedTime = closedTime;
                tblObj.ClosedMsg = closedMsg;
                tblObj.SubmittedTime = submittedTime;
                tblObj.SubmittedVia = SubmittedVia;

                entityObj.tblCitizensConnectDumps.Add(tblObj);
                entityObj.SaveChanges();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //TODO
            }
        }

        /// <summary>
        /// GetNUm takes the raw text and returns double 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public int GetNum(string elem)
        {
            int y;
            string parsedText = elem.Replace("View Reports - ", "").Replace("found", "").Replace(" ", "").Replace(",", "");

            parsedText = parsedText.Replace("(", "").Replace(")", "");

            Int32.TryParse(parsedText, out y);

            return y;
        }

        /// <summary>
        /// Get DateID
        /// </summary>
        private void GetDateID()
        {
            //try
            //{


            //    string formatedDate = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.GetCultureInfo("en-US"));

            //    int intDate;
            //    bool isNum = Int32.TryParse(formatedDate, out intDate);

            //    Debug.Assert(isNum == true, "Date is not in proper order");

            //    if (isNum)
            //    {
            //        var query = (from dates in _entity.DimDates
            //                     where dates.DateSK == intDate
            //                     select dates.DateSK).SingleOrDefault();

            //        _intDateID = query;
            //    }


            //    #endregion insertGetDateID
            //}
            //catch (Exception ex)
            //{

            //    //MessageBox.Show(ex.Message);
            //}
        }
    }
}
