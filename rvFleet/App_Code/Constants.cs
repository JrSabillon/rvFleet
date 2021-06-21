using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace rvFleet.App_Code
{
    public static class Constants
    {
        public const string App_Error = "APP Error";
        public const string DB_Error = "DB Error";
        public const string User_Active_Code = "001";
        /// <summary>
        /// Path donde se guardaran los archivos de las facturas
        /// </summary>
        public static string FacturasPath = ConfigurationManager.AppSettings["FacturasPath"];
    }
}