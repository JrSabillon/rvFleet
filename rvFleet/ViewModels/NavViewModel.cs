using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace rvFleet.ViewModels
{
    public class NavViewModel
    {
        
        public List<privilegio> GetPrivilegios(string UserId)
        {
            List<privilegio> privilegios = new List<privilegio>();
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;user id=root;password=desarrollo;persistsecurityinfo=True;database=rvseguridad;");
            
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

                return privilegios;
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }
    }
}