using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using rvFleet.App_Code;
using System.Data.Entity;
using MySql.Data.MySqlClient;

namespace rvFleet.ViewModels
{
    public class LogsViewModel
    {
        public LogsViewModel()
        {

        }

        public List<bitacoravehiculo> GetBitacoravehiculos(string VehPlaca)
        {
            try
            {
                List<bitacoravehiculo> data = new List<bitacoravehiculo>();

                using (var context = new rvfleetEntities())
                {
                    if (!string.IsNullOrEmpty(VehPlaca))
                        data = context.bitacoravehiculo.Where(x => x.bitacoraPlaca == VehPlaca).Include(x => x.bitacoracomentario1).ToList();
                    else
                        data = context.bitacoravehiculo.Include(x => x.bitacoracomentario1).ToList();
                }

                using (var context = new rvseguridadEntities1())
                {
                    var tipos = context.opcionrecurso.Where(x => x.IdGrupoRecurso.Equals("VEHIC_BIT_TIP")).ToList();

                    foreach (var item in data)
                    {
                        item.bitacoraTipoDescripcion = tipos.Where(x => x.IdOpcionRecurso.Equals(item.bitacoraTipo)).FirstOrDefault().NombreOpcionRecurso;
                    }
                }

                return data;
            }
            catch(Exception exc)
            {
                throw new ApplicationException(string.Format("{0} - {1}", Constants.App_Error, exc.Message));
            }
        }

        public List<opcionrecurso> getTipoBitacoras()
        {
            try
            {
                using (var context = new rvseguridadEntities1())
                {
                    var data = context.opcionrecurso.Where(x => x.IdGrupoRecurso.Equals("VEHIC_BIT_TIP")).ToList();
                    
                    return data;
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(string.Format("{0} - {1}", Constants.App_Error, exc.Message));
            }
        }

        public void AddLog(bitacoravehiculo data)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    context.bitacoravehiculo.Add(data);
                    context.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(string.Format("{0} - {1}", Constants.App_Error, exc.Message));
            }
        }

        public void AddLogComment(bitacoracomentario data)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var comments = context.bitacoracomentario.Where(x => x.bitacoraId == data.bitacoraId).ToList();
                    data.bitacoraComId = comments.Count + 1;
                    context.bitacoracomentario.Add(data);

                    context.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(string.Format("{0} - {1}", Constants.App_Error, exc.Message));
            }
        }

        public bool UserCanSeeLogs(string UserId)
        {
            //MySqlConnection conn = new MySqlConnection(Constants.SecurityStaticConnection);
            
            try
            {
                using (var context = new rvseguridadEntities1())
                {
                    var user = context.usuario.Where(x => x.IdUsuario == UserId).FirstOrDefault();

                    return user.IdPosicion != 20;
                }

                //string query = $"SELECT Count(*) FROM rolusuario INNER JOIN rolprivilegio " +
                //    $"ON rolusuario.IdRol = rolprivilegio.IdRol " +
                //    $"WHERE rolusuario.IdUsuario = '{UserId}' AND IdPrivilegio = 'VEHIC_LOG'";
                //MySqlCommand cmd = new MySqlCommand(query, conn);
                //conn.Open();

                //var result = Convert.ToInt32(cmd.ExecuteScalar());

                //return result > 0;
            }
            catch(Exception exc)
            {
                throw new ApplicationException(string.Format("{0} - {1}", Constants.App_Error, exc.Message));
            }
        }

        public controlvehiculosrespuestadetalle GetDetalleInspeccion(int IdControl, int CodigoVehiculo)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.controlvehiculosrespuestadetalle.Where(x => x.IdControl == IdControl && x.CodigoVehiculo == CodigoVehiculo).FirstOrDefault();

                    return data;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}