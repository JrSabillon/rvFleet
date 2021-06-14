using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.ViewModels
{
    public class RubrosViewModel
    {
        public List<rubros> GetRubros()
        {
            try
            {
                List<rubros> rubros = new List<rubros>();

                using (var context = new rvfleetEntities())
                {
                    rubros = context.rubros.ToList();
                }

                return rubros;
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