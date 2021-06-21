using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using rvFleet.POCO;

namespace rvFleet.ViewModels
{
    public class VehiclesViewModel
    {
        public List<vehiculos> GetVehiculos()
        {
            try
            {
                List<vehiculos> vehiculos = new List<vehiculos>();

                using (var context = new rvfleetEntities())
                {
                    vehiculos = context.vehiculos.ToList();
                }

                return vehiculos;
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

        public vehiculos GetVehiculo(string VehPlaca)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var vehicle = context.vehiculos.Where(x => x.VehPlaca.Equals(VehPlaca))
                        .FirstOrDefault();

                    return vehicle;
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

        public vehiclefulldata GetVehiclefulldata(string VehPlaca)
        {
            try
            {
                var vehicleData = this.GetVehiclesCompleteData().Where(x => x.VehPlaca.Equals(VehPlaca))
                    .FirstOrDefault();

                return vehicleData;
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

        public List<vehiclefulldata> GetVehiclesCompleteData()
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.vehiclefulldata.ToList();

                    //var data = from vehicle in context.vehiculos
                    //           join user in context.usuarios on vehicle equals user.IdUsuario into gj
                    //           from vehuser in gj.DefaultIfEmpty()
                    //           select new VehicleModel { 
                    //               VehPlaca = vehicle.VehPlaca, 
                    //               VehColor = vehicle.VehColor, 
                    //               VehAno = vehicle.VehAno, 
                    //               NombreEncargado = vehuser?.NombreUsuario ?? string.Empty 
                    //           }

                    return data;
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

        public vehiculos GetVehiculoUsuario(string UserId)
        {
            try
            {
                vehiculos vehiculo = new vehiculos();

                using (var context = new rvfleetEntities())
                {
                    vehiculo = context.vehiculos.Where(x => x.VehCodigoUsuario.Equals(UserId)).FirstOrDefault();
                }

                return vehiculo;
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

        public bool UserHasVehicle(string UserId)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var flag = context.vehiculos.Where(x => x.VehCodigoUsuario.Equals(UserId)).Count() > 0;

                    return flag;
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

        public vehiclefulldata CreateVehicle(vehiculos vehicle)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    context.vehiculos.Add(vehicle);
                    context.SaveChanges();
                    var newVehicle = context.vehiclefulldata.Where(x => x.VehPlaca.Equals(vehicle.VehPlaca)).FirstOrDefault();

                    return newVehicle;
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

        public vehiculos UpdateVehicle(vehiculos vehicle)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var CurrentVehicle = context.vehiculos.Where(x => x.VehCodigoVehiculo.Equals(vehicle.VehCodigoVehiculo)).FirstOrDefault();
                    CurrentVehicle.VehPlaca = vehicle.VehPlaca;
                    CurrentVehicle.VehVIN = vehicle.VehVIN;
                    CurrentVehicle.VehMotor = vehicle.VehMotor;
                    CurrentVehicle.VehCilindraje = vehicle.VehCilindraje;
                    CurrentVehicle.VehAno = vehicle.VehAno;
                    CurrentVehicle.VehColor = vehicle.VehColor;
                    CurrentVehicle.VehMarca = vehicle.VehMarca;
                    CurrentVehicle.VehModelo = vehicle.VehModelo;
                    CurrentVehicle.VehFechaMatricula = vehicle.VehFechaMatricula;
                    CurrentVehicle.VehCodigoAresSun = vehicle.VehCodigoAresSun;
                    CurrentVehicle.VehNumeroPoliza = vehicle.VehNumeroPoliza;
                    CurrentVehicle.VehCodigoProveedor = vehicle.VehCodigoProveedor;
                    CurrentVehicle.VehCodigoEmpresa = vehicle.VehCodigoEmpresa;
                    CurrentVehicle.VehCodigoUsuario = vehicle.VehCodigoUsuario;
                    context.SaveChanges();

                    return CurrentVehicle;
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