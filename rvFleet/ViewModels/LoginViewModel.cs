using rvFleet.App_Code;
using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        /// Validar que el usuario existe, tenga las credenciales correctas y este activo
        /// </summary>
        /// <param name="UserId">Id del usuario</param>
        /// <param name="Password">Contraseña actual encriptada</param>
        /// <returns>Respuesta booleana que indica si el usuario a sido validado correctamente.</returns>
        public void LoginUser(string UserId, string Password, out bool IsValidated, out string Message, usuario user)
        {
            try
            {
                IsValidated = false;
                Message = string.Empty;
                
                using (var context = new rvseguridadEntities1())
                {
                    var result = context.usuario.Where(x => x.IdUsuario.Equals(UserId) && x.Contrasena.Equals(Password)).FirstOrDefault();
                     
                    if(result != null)
                    {
                        ///Validó correctamente al usuario pero falta por hacer otras verificaciones (si se encuentra activo).
                        if (!result.Estado.Equals(Constants.User_Active_Code))
                        {
                            ///Si el estado no es 001 entonces el usuario no puede ingresar porque no esta activo.
                            Message = "Usuario inactivo.";
                        }
                        else
                        {
                            ///Obtener datos de la empresa para usarlas en la app
                            user.IdEmpresa = result.IdEmpresa;

                            IsValidated = true;
                        }
                    }
                    else
                    {
                        Message = "Usuario y/o contraseña incorrectas.";
                    }
                }
            }
            catch(Exception exc)
            {
                throw new ApplicationException($"{Constants.App_Error} - {exc.Message}");
            }
        }
    }
}