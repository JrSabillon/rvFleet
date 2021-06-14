using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.App_Code;
using rvFleet.Models;
using MySql.Data.MySqlClient;

namespace rvFleet.ViewModels
{
    public class OrdersViewModel
    {
        /// <summary>
        /// Obtener todas las facturas registradas en el sistema.
        /// </summary>
        /// <returns></returns>
        public List<facturas> GetFacturas()
        {
            try
            {
                List<facturas> facturas = new List<facturas>();

                using (var context = new rvfleetEntities())
                {
                    facturas = context.facturas.Where(x => !string.IsNullOrEmpty(x.FacNumeroFactura)).ToList();
                }

                return facturas;
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

        /// <summary>
        /// Obtener lista de elementos que contiene una orden/factura.
        /// </summary>
        /// <param name="codigoOrden">codigo de la orden ligada a la factura.</param>
        /// <returns></returns>
        public List<detallefactura> GetDetallefacturas(int codigoOrden)
        {
            try
            {
                List<detallefactura> detallefacturas = new List<detallefactura>();

                using (var context = new rvfleetEntities())
                {
                    detallefacturas = context.detallefactura.Where(x => x.DetCodigoOrden.Equals(codigoOrden)).ToList();
                }

                return detallefacturas;
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

        /// <summary>
        /// Obtener todas las ordenes que no han sido consolidadas como facturas.
        /// </summary>
        /// <returns></returns>
        public List<facturas> GetOrdenes()
        {
            try
            {
                List<facturas> ordenes = new List<facturas>();

                using (var context = new rvfleetEntities())
                {
                    ordenes = context.facturas.Where(x => string.IsNullOrEmpty(x.FacNumeroFactura)).ToList();
                }

                return ordenes;
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

        /// <summary>
        /// Obtener todas las ordenes y facturas.
        /// </summary>
        /// <returns></returns>
        public List<facturas> GetOrdenesFacturas()
        {
            try
            {
                List<facturas> ordenes = new List<facturas>();

                using (var context = new rvfleetEntities())
                {
                    ordenes = context.facturas.ToList();
                }

                return ordenes;
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

        public proveedor GetProveedor(int codigoProveedor)
        {
            try
            {
                proveedor proveedor = new proveedor();

                using (var context = new rvfleetEntities())
                {
                    proveedor = context.proveedor.Where(x => x.ProCodigoProveedor.Equals(codigoProveedor)).FirstOrDefault();
                }

                return proveedor;
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

        public void CreateFactura(facturas factura)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    context.facturas.Add(factura);
                    context.SaveChanges();

                    var FacCodigoOrden = factura.FacCodigoOrden;
                }
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