using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using rvFleet.App_Code;
using MySql.Data.MySqlClient;

namespace rvFleet.ViewModels
{
    public class ArchivoFacturaViewModel
    {
        public void SaveFile(archivofactura archivofactura)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    context.archivofactura.Add(archivofactura);
                    context.SaveChanges();
                }
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

        public int GetNextId(int FacCodigoOrden)
        {
            try
            {
                int nextId = 0;

                using (var context = new rvfleetEntities())
                {
                    if(context.archivofactura.Where(x => x.FacCodigoOrden.Equals(FacCodigoOrden)).Count() > 0)
                    {
                        var MaxId = context.archivofactura.Where(x => x.FacCodigoOrden.Equals(FacCodigoOrden)).Max(x => x.FacArchivoCodigo);
                        nextId = MaxId + 1;
                    }
                    else
                    {
                        nextId = 1;
                    }
                }

                return nextId;
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

        public archivofactura RemoveArchivoFacura(int FacCodigoOrden, int FacArchivoCodigo)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var archivoFactura = context.archivofactura.Where(x => x.FacCodigoOrden.Equals(FacCodigoOrden) && x.FacArchivoCodigo.Equals(FacArchivoCodigo))
                        .FirstOrDefault();

                    context.archivofactura.Remove(archivoFactura);
                    context.SaveChanges();

                    return archivoFactura;
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