using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;

namespace rvFleet.ViewModels
{
    public class VehiclesViewModel
    {
        public List<vehiculos> GetVehiculos()
        {
            try
            {
                List<vehiculos> vehiculos = new List<vehiculos>();

                using (var context = new rvfleetEntities())
                {
                    vehiculos = context.vehiculos.ToList();
                }

                return vehiculos;
            }
            catch(MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch(Exception exc)
            {
                throw new ApplicationException($"{Constants.App_Error} - {exc.Message}");
            }
        }
    }
}