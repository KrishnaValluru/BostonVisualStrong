using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtractBostonCitizensConnect
{
    class Program
    {
        /// <summary>
        /// to extract Boston Citizens Connect data from
        /// 
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ExtractData objExtract = new ExtractData();

            objExtract.StartProcess();
        }
    }
}
