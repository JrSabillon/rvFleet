using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using System.Data.Entity;

namespace rvFleet.ViewModels
{
    public class FacturasViewModel
    {
        public facturas GetFacturas(int FacCodigoOrden)
        {
            try
            {
                facturas facturas = new facturas();

                using (var context = new rvfleetEntities())
                {
                    facturas = context.facturas.Where(x => x.FacCodigoOrden.Equals(FacCodigoOrden)).Include(x => x.proveedor).Include(x => x.detallefactura).FirstOrDefault();
                }

                return facturas;
            }
            catch (MySqlException dbExc)
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