using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;

namespace rvFleet.ViewModels
{
    public class ProvidersViewModel
    {
        public List<proveedor> GetProveedors()
        {
            try
            {
                List<proveedor> proveedors = new List<proveedor>();

                using (var context = new rvfleetEntities())
                {
                    proveedors = context.proveedor.ToList();
                }

                return proveedors;
            }
            catch (MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch (Exception exc)
            {
                throw new ApplicationException($"{Constants.App_Error} - {exc.Message}");
            }
        }
    }
}