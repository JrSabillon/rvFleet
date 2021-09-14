using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using rvFleet.App_Code;

namespace rvFleet.ViewModels
{
    public class NavViewModel
    {

        public List<privilegio> GetPrivilegios(string UserId)
        {
            List<privilegio> privilegios = new List<privilegio>();
            MySqlConnection conn = new MySqlConnection(Constants.SecurityStaticConnection);
            
            try
            {
                string query = $"SELECT IdPrivilegio, NombrePrivilegio, DescripcionPrivilegio, EstadoPrivilegio, NivelPrivilegio, IFNULL(PadrePrivilegio, ''), IFNULL(URL, ''), IFNULL(Parametros, ''), IFNULL(Icono, '') FROM privilegio WHERE IdPrivilegio IN( " +
                    $"SELECT DISTINCT IdPrivilegio FROM rolusuario a INNER JOIN rolprivilegio b " +
                    $"ON a.IdRol = b.IdRol " +
                    $"WHERE IdUsuario = '{UserId}')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        privilegios.Add(new privilegio
                        {
                            IdPrivilegio = dr.GetString(0),
                            NombrePrivilegio = dr.GetString(1),
                            DescripcionPrivilegio = dr.GetString(2),
                            EstadoPrivilegio = dr.GetInt32(3).Equals(1),
                            NivelPrivilegio = dr.GetInt32(4),
                            PadrePrivilegio = dr.GetString(5),
                            URL = dr.GetString(6),
                            Parametros = dr.GetString(7),
                            Icono = dr.GetString(8)
                        });
                    }
                }

                return privilegios.Where(x => x.EstadoPrivilegio.Value).ToList();
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }
    }
}