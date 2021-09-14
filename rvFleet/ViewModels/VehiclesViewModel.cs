using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using rvFleet.POCO;
using System.Data.Entity;

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

        /// <summary>
        /// Obtener los datos completos del vehiculo por su codigo
        /// </summary>
        /// <param name="VehCodigo">Codigo del vehiculo</param>
        /// <returns></returns>
        public vehiculos GetVehiculoById(int VehCodigo)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var vehicle = context.vehiculos.Where(x => x.VehCodigoVehiculo.Equals(VehCodigo))
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
                    CurrentVehicle.VehUrlFotoRevision = vehicle.VehUrlFotoRevision ?? CurrentVehicle.VehUrlFotoRevision;
                    CurrentVehicle.VehFotoFrontal = vehicle.VehFotoFrontal ?? CurrentVehicle.VehFotoFrontal;
                    CurrentVehicle.VehFotoLateralDerecha = vehicle.VehFotoLateralDerecha ?? CurrentVehicle.VehFotoLateralDerecha;
                    CurrentVehicle.VehFotoLateralIzquierda = vehicle.VehFotoLateralIzquierda ?? CurrentVehicle.VehFotoLateralIzquierda;
                    CurrentVehicle.VehFotoTrasera = vehicle.VehFotoTrasera ?? CurrentVehicle.VehFotoTrasera;
                    CurrentVehicle.VehFotoMotor = vehicle.VehFotoMotor ?? CurrentVehicle.VehFotoMotor;
                    CurrentVehicle.VehFotoInterior = vehicle.VehFotoInterior ?? CurrentVehicle.VehFotoInterior;
                    CurrentVehicle.VehKilometrajeActualizado = CurrentVehicle.VehKilometraje != vehicle.VehKilometraje ? DateTime.Now : CurrentVehicle.VehKilometrajeActualizado;
                    CurrentVehicle.VehKilometraje = vehicle.VehKilometraje;

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

        public List<GetVehicleGraphData_Result> GetGraphData(string VehPlaca)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var graphData = context.GetVehicleGraphData(VehPlaca).ToList();
                    foreach (var item in graphData)
                    {
                        if(item.Porcentaje > 70)
                        {
                            item.BackgroundColor = Colors.Green;
                            item.BorderColor = Colors.GreenBorder;
                        }
                        else if(item.Porcentaje > 30)
                        {
                            item.BackgroundColor = Colors.Yellow;
                            item.BorderColor = Colors.YellowBorder;
                        }
                        else
                        {
                            item.BackgroundColor = Colors.Red;
                            item.BorderColor = Colors.RedBorder;
                        }
                    }

                    return graphData;
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

        public List<controlvehiculospregunta> GetPreguntas()
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var preguntas = context.controlvehiculospregunta.Where(x => x.Estado.Value).OrderBy(x => x.Orden).ToList();

                    return preguntas;
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

        public void SaveInspectionAnswer(controlvehiculosrespuesta respuesta, DateTime Fecha)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    respuesta.Fecha = Fecha;
                    respuesta.CodigoUsuario = BaseViewModel.GetUserData().IdUsuario;

                    context.controlvehiculosrespuesta.Add(respuesta);
                    context.SaveChanges();
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

        public List<controlvehiculosrespuesta> GetRespuestasInspeccion(int CodigoVehiculo, DateTime Fecha)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.controlvehiculosrespuesta.Include(x => x.controlvehiculospregunta)
                        .Where(x => x.CodigoVehiculo == CodigoVehiculo && x.Fecha == Fecha).ToList();

                    return data;
                }
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }

        public List<InspectionsTable> GetRespuestasGrouped(string VehPlaca)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = new List<InspectionsTable>();

                    if (!string.IsNullOrEmpty(VehPlaca))
                    {
                        data = context.controlvehiculosrespuesta
                            .Join(context.vehiculos, 
                                c => c.CodigoVehiculo, 
                                v => v.VehCodigoVehiculo, 
                                (c, v) => new InspectionsTable { VehPlaca = v.VehPlaca, Fecha = c.Fecha, VehCodigoVehiculo = v.VehCodigoVehiculo, CodigoUsuario = c.CodigoUsuario })
                            .Where(x => x.VehPlaca == VehPlaca)
                            .GroupBy(x => new { x.VehCodigoVehiculo, x.Fecha, x.VehPlaca, x.CodigoUsuario })
                            .Select(x => new InspectionsTable { VehPlaca = x.Key.VehPlaca, Fecha = x.Key.Fecha, VehCodigoVehiculo = x.Key.VehCodigoVehiculo, CodigoUsuario = x.Key.CodigoUsuario })
                            .ToList();
                    }
                    else
                    {
                        data = context.controlvehiculosrespuesta
                            .Join(context.vehiculos,
                                c => c.CodigoVehiculo,
                                v => v.VehCodigoVehiculo,
                                (c, v) => new InspectionsTable { VehPlaca = v.VehPlaca, Fecha = c.Fecha, VehCodigoVehiculo = v.VehCodigoVehiculo, CodigoUsuario = c.CodigoUsuario })
                            .GroupBy(x => new { x.VehCodigoVehiculo, x.Fecha, x.VehPlaca, x.CodigoUsuario })
                            .Select(x => new InspectionsTable { VehPlaca = x.Key.VehPlaca, Fecha = x.Key.Fecha, VehCodigoVehiculo = x.Key.VehCodigoVehiculo, CodigoUsuario = x.Key.CodigoUsuario })
                            .ToList();
                    }

                    return data;
                }
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }
    }
}