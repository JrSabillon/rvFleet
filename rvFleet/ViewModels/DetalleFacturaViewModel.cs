using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.App_Code;
using MySql.Data.MySqlClient;
using System.Data.Entity;

namespace rvFleet.ViewModels
{
    public class DetalleFacturaViewModel
    {
        public List<detallefactura> GetDetallefactura(int DetCodigoOrden)
        {
            try
            {
                List<detallefactura> detallesfactura = new List<detallefactura>();

                using (var context = new rvfleetEntities())
                {
                    detallesfactura = context.detallefactura.Where(x => x.DetCodigoOrden.Equals(DetCodigoOrden)).Include(x => x.facturas)
                        .Include(x => x.facturas.detallefactura).Include(x => x.facturas.proveedor).Include(x => x.facturas.proveedor.facturas).ToList();
                }

                return detallesfactura;
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