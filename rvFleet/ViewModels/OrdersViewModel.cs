using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.App_Code;
using rvFleet.Models;
using MySql.Data.MySqlClient;
using System.Data.Entity;

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
        /// Obtener detalles de la factura seleccionada.
        /// </summary>
        /// <param name="FacCodigoOrden">Id de la orden</param>
        /// <returns></returns>
        public facturas GetFactura(int FacCodigoOrden)
        {
            try
            {
                facturas facturas = new facturas();

                using (var context = new rvfleetEntities())
                {
                    facturas = context.facturas.Where(x => x.FacCodigoOrden.Equals(FacCodigoOrden)).Include(x => x.proveedor).Include(x => x.detallefactura)
                        .Include(x => x.archivofactura).FirstOrDefault();
                }

                return facturas;
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
                    ordenes = context.facturas.Where(x => string.IsNullOrEmpty(x.FacNumeroFactura)).Include(x => x.detallefactura).ToList();
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
                    //var FacCodigoOrden = factura.FacCodigoOrden;
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

        public facturas UpdateFactura(facturas factura)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var CurrentFactura = context.facturas.Find(factura.FacCodigoOrden);
                    var total = factura.detallefactura.Sum(x => x.DetValor);
                    CurrentFactura.FacCodigoProveedor = factura.FacCodigoProveedor;
                    CurrentFactura.FacUsuarioPago = factura.FacUsuarioPago;
                    CurrentFactura.FacFechaOrden = factura.FacFechaOrden;
                    CurrentFactura.FacFechaFactura = factura.FacFechaFactura;
                    CurrentFactura.FacNumeroFactura = factura.FacNumeroFactura;
                    CurrentFactura.FacKilometraje = factura.FacKilometraje;
                    CurrentFactura.FacComentario = factura.FacComentario;
                    CurrentFactura.FacAplicaImpuesto = factura.FacAplicaImpuesto;
                    CurrentFactura.FacValorFactura = factura.FacAplicaImpuesto.Value ? total * 0.15 + total : total;
                    CurrentFactura.detallefactura.Clear();
                    context.SaveChanges();

                    ///Update detalleFactura
                    CurrentFactura.detallefactura = factura.detallefactura;
                    context.SaveChanges();
                    return CurrentFactura;
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