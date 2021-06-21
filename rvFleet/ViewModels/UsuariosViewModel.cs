using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using System.Data.Entity;

namespace rvFleet.ViewModels
{
    public class UsuariosViewModel
    {
        public usuario GetUsuario(string UserId)
        {
            try
            {
                usuario user = new usuario();

                using (var context = new rvseguridadEntities1())
                {
                    user = context.usuario.Find(UserId);

                    return user;
                }
            }
            catch(MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch(Exception exc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {exc.Message}");
            }
        }

        public List<usuario> GetUsuarios()
        {
            try
            {
                List<usuario> usuarios = new List<usuario>();

                using (var context = new rvseguridadEntities1())
                {
                    usuarios = context.usuario.ToList();
                }

                return usuarios;
            }
            catch (MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch (Exception exc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {exc.Message}");
            }
        }

        public usuario SaveUsuario(usuario usuario)
        {
            try
            {
                using (var context = new rvseguridadEntities1())
                {
                    var CurrentUser = context.usuario.Find(usuario.IdUsuario);
                    CurrentUser.NombreUsuario = usuario.NombreUsuario;
                    CurrentUser.IdentidadUsuario = usuario.IdentidadUsuario;
                    CurrentUser.Correo = usuario.Correo;
                    context.SaveChanges();

                    return CurrentUser;
                }
            }
            catch (MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch (Exception exc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {exc.Message}");
            }
        }
    }
}