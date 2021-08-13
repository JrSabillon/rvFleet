using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using rvFleet.App_Code;
using System.Data.Entity;

namespace rvFleet.ViewModels
{
    public class LogsViewModel
    {
        public LogsViewModel()
        {

        }

        public List<bitacoravehiculo> GetBitacoravehiculos()
        {
            try
            {
                List<bitacoravehiculo> data = new List<bitacoravehiculo>();

                using (var context = new rvfleetEntities())
                {
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
    }
}