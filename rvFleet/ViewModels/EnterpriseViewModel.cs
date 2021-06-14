using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;

namespace rvFleet.ViewModels
{
    public class EnterpriseViewModel
    {
        public empresa GetEmpresa(string codigoEmpresa)
        {
            try
            {
                empresa empresa = new empresa();

                using (var context = new rvseguridadEntities1())
                {
                    empresa = context.empresa.Where(x => x.IdEmpresa.Equals(codigoEmpresa)).FirstOrDefault();
                }

                return empresa;
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