using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;
using rvFleet.App_Code;
using MySql.Data.MySqlClient;
using System.Data.Entity;

namespace rvFleet.ViewModels
{
    public class KilometrajeHistoricoViewModel
    {
        /// <summary>
        /// Metodo para verificar si el usuario ya subio el kilometraje del vehiculo el dia de hoy.
        /// </summary>
        /// <param name="CodigoVehiculo">Codigo del vehículo</param>
        /// <returns></returns>
        public bool KilometrajeUploaded(int CodigoVehiculo)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var today = DateTime.Today;
                    var data = context.kilometrajehistorico.Where(x => x.KilCodigoVehiculo.Equals(CodigoVehiculo) && 
                    x.KilFechaIngreso.Year == today.Year && x.KilFechaIngreso.Month == today.Month && x.KilFechaIngreso.Day == today.Day).ToList();

                    return data.Count > 0;
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

        public kilometrajehistorico UploadKilometraje(kilometrajehistorico Kilometraje)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    if(context.kilometrajehistorico.Where(x => x.KilFechaIngreso.Equals(Kilometraje.KilFechaIngreso) && x.KilCodigoVehiculo.Equals(Kilometraje.KilCodigoVehiculo)).Count() == 0)
                    {
                        var NewKilometraje = context.kilometrajehistorico.Add(Kilometraje);
                        var vehicle = context.vehiculos.Where(x => x.VehCodigoVehiculo.Equals(Kilometraje.KilCodigoVehiculo)).FirstOrDefault();
                        vehicle.VehKilometraje = Kilometraje.KilKilometraje;
                        vehicle.VehKilometrajeActualizado = DateTime.Now;
                        context.SaveChanges();

                        return NewKilometraje;
                    }

                    return new kilometrajehistorico();
                }
            }
            catch (MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch (Exception exc)
            {
                throw new ApplicationException($"{Constants.App_Error} / UploadKilometraje - {exc.Message}");
            }
        }
    }
}