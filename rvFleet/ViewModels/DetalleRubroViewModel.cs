using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.ViewModels
{
    public class DetalleRubroViewModel
    {
        public List<rubrodetalle> GetRubrodetalles(int CodigoRubro)
        {
            try
            {
                List<rubrodetalle> rubrodetalles = new List<rubrodetalle>();

                using (var context = new rvfleetEntities())
                {
                    rubrodetalles = context.rubrodetalle.Where(x => x.CodigoRubro.Equals(CodigoRubro)).ToList();
                }

                rubrodetalles.Add(new rubrodetalle
                {
                    CodigoDetalle = 0,
                    CodigoRubro = 0,
                    NombreDetalle = "Otros",
                });

                return rubrodetalles;
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