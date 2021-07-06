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
        public static string SecurityStaticConnection = ConfigurationManager.ConnectionStrings["rvSeguridadStaticConnection"].ConnectionString;
        /// <summary>
        /// Path donde se guardaran los archivos de las facturas
        /// </summary>
        public static string FacturasPath = ConfigurationManager.AppSettings["FacturasPath"];
        /// <summary>
        /// Path donde se guardaran los archivos de los vehiculos (Kilometrajes, fotos del vehiculo, etc).
        /// </summary>
        public static string VehiculosPath = ConfigurationManager.AppSettings["VehiculosPath"];
    }

    public static class Colors
    {
        public const string Green = "rgba(75, 192, 192, 0.2)";
        public const string GreenBorder = "rgba(75, 192, 192, 1)";
        public const string Yellow = "rgba(255, 206, 86, 0.2)";
        public const string YellowBorder = "rgba(255, 206, 86, 1)";
        public const string Red = "rgba(255, 99, 132, 0.2)";
        public const string RedBorder = "rgba(255, 99, 132, 1)";
    }
}