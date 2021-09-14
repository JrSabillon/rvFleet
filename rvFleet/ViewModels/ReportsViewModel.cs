using rvFleet.POCO.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using rvFleet.App_Code;
using System.Configuration;
using rvFleet.Models;
using System.Data.Entity;

namespace rvFleet.ViewModels
{
    public class ReportsViewModel
    {
        string _ConnectionString;
        public ReportsViewModel()
        {
            _ConnectionString = ConfigurationManager.ConnectionStrings["rvFleetStaticConnection"].ConnectionString;
        }

        public List<vehiclecosts> GetVehicleCosts()
        {
            try
            {
                List<vehiclecosts> vehicleCosts = new List<vehiclecosts>();

                //using (var context = new rvfleetEntities())
                //{
                //    vehicleCosts = context.vehiclecosts.ToList();
                //}

                using (MySqlConnection conn = new MySqlConnection(_ConnectionString))
                {
                    string procedure = "spGetVehiclesCostResume";
                    MySqlCommand cmd = new MySqlCommand(procedure, conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vehicleCosts.Add(new vehiclecosts
                            {
                                Total = reader.GetDouble(0),
                                DetPlacaVehiculo = reader.GetString(1),
                                PromedioMensual = reader.GetDouble(2)
                            });
                        }
                    }
                    conn.Close();
                }

                return vehicleCosts;
            }
            catch(MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch(Exception exc)
            {
                throw exc;
                //throw new ApplicationException($"{Constants.App_Error} - {exc.Message}");
            }
        }

        public List<recommendedmaintenance> GetRecommendedMaintenance()
        {
            try
            {
                List<recommendedmaintenance> recommendedmaintenances = new List<recommendedmaintenance>();

                //using (var context = new rvfleetEntities())
                //{
                //    recommendedmaintenances = context.recommendedmaintenance.ToList();
                //}
                using (MySqlConnection conn = new MySqlConnection(_ConnectionString))
                {
                    string query = "SELECT * FROM recommendedmaintenance";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recommendedmaintenances.Add(new recommendedmaintenance
                            {
                                CodigoRubro = reader.GetInt32(0),
                                NombreRubro = reader.GetString(1),
                                DetPlacaVehiculo = reader.GetString(2),
                                VehCodigoVehiculo = reader.GetInt32(3),
                                DistanciaCambio = reader.GetInt32(4),
                                KilometrajeFacturado = reader.GetInt32(5),
                                KilometrajeActual = reader.GetInt32(6),
                                DistanciaRecorrida = reader.GetInt32(7),
                                VidaRestante = reader.GetInt32(8),
                                DetCodigoOrden = reader.GetInt32(9),
                                FacFechaOrden = reader.GetDateTime(10),
                                Prioridad = reader.GetInt32(11)
                            });
                        }
                    }
                }

                this.RecommendedMaintenanceNotRegistered(recommendedmaintenances);

                return recommendedmaintenances;
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

        public void RecommendedMaintenanceNotRegistered(List<recommendedmaintenance> model)
        {
            try
            {
                //using (var context = new rvfleetEntities())
                //{
                //    var data = context.spGetRubrosNoFacturadosVehiculo().ToList();

                //    foreach (var item in data)
                //    {
                //        model.Add(item);
                //    }
                //}

                using (MySqlConnection conn = new MySqlConnection(_ConnectionString))
                {
                    string query = "spGetRubrosNoFacturadosVehiculo";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Add(new recommendedmaintenance
                            {
                                CodigoRubro = reader.GetInt32(0),
                                NombreRubro = reader.GetString(1),
                                DetPlacaVehiculo = reader.GetString(2),
                                VehCodigoVehiculo = reader.GetInt32(3),
                                DistanciaCambio = reader.GetInt32(4),
                                KilometrajeFacturado = reader.GetInt32(5),
                                KilometrajeActual = reader.GetInt32(6),
                                DistanciaRecorrida = reader.GetInt32(7),
                                VidaRestante = reader.GetInt32(8),
                                DetCodigoOrden = reader.GetInt32(9),
                                //FacFechaOrden = reader.GetDateTime(10),
                                Prioridad = reader.GetInt32(11)
                            });
                        }
                    }
                    conn.Close();
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

        public void CarInventory(out int Assigned, out int Unassigned, out int total)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var vehicles = context.vehiculos.ToList();
                    Assigned = vehicles.Where(x => !string.IsNullOrEmpty(x.VehCodigoUsuario)).Count();
                    Unassigned = vehicles.Where(x => string.IsNullOrEmpty(x.VehCodigoUsuario)).Count();

                    total = Assigned + Unassigned;
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

        public List<GetVehicleAnualCostsGraphData_Result> VehicleAnualCosts(int YearFilter)
        {
            try
            {
                List<GetVehicleAnualCostsGraphData_Result> data = new List<GetVehicleAnualCostsGraphData_Result>();

                using (var context = new rvfleetEntities())
                {
                    data = context.GetVehicleAnualCostsGraphData(YearFilter).ToList();
                }

                return data;
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

        public List<kilometrajeporvehiculoanoactual> GetKilometrajePorVehiculoAnoActual()
        {
            try
            {
                List<kilometrajeporvehiculoanoactual> data = new List<kilometrajeporvehiculoanoactual>();

                //using (var context = new rvfleetEntities())
                //{
                //    data = context.kilometrajeporvehiculoanoactual.ToList();
                //}
                using (MySqlConnection conn = new MySqlConnection(_ConnectionString))
                {
                    string query = "SELECT * FROM kilometrajeporvehiculoanoactual";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new kilometrajeporvehiculoanoactual
                            {
                                KilCodigoVehiculo = reader.GetInt32(0),
                                KilFechaIngreso = reader.GetDateTime(1),
                                KilFechaIngreso_Mes = reader.GetInt32(2),
                                KilKilometraje = reader.GetInt32(3),
                                VehPlaca = reader.GetString(4),
                                VehColorIdentificador = reader.GetString(5)
                            });
                        }
                    }
                }

                return data;
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

        public List<GetVehicleCosts_Result> GetVehicleCosts_Filtered(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.GetVehicleCosts(startDate, endDate).ToList();

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

        //public List<vehiculos> GetVehiclesTableCost(DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        using (var context = new rvfleetEntities())
        //        {
        //            //var resumenCostos = context.resumencostosvehiculos.ToList().Where(x => x.);
        //            var vehicles = context.vehiculos.ToList();

        //            vehicles.ForEach( vehicle => {
        //                vehicle.Costs = context.spGetVehiclesCostTableValues(vehicle.VehPlaca, startDate, endDate).ToList();
        //            });

        //            return vehicles;

        //            //return resumenCostos;
        //        }
        //    }
        //    catch(Exception exc)
        //    {
        //        throw exc;
        //    }
        //}

        /// <summary>
        /// Obtener tabla de costos incluyendo los meses que no han habido facturas.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<spGetVehicleCostsFiltered_Result> GetVehicleCostsFiltered(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.spGetVehicleCostsFiltered(startDate, endDate).ToList();

                    return data;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<spGetVehicleCostsFiltered_Result> GetRubrosTableCost(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.spGetRubrosTableCost(startDate, endDate).ToList();

                    return data;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<spGetVehicleCostsFiltered_Result> GetProveedoresTableCost(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.spGetProveedoresTableCost(startDate, endDate).ToList();

                    return data;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<GetPartsCost_Result> GetPartsCost_Filtered(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var data = context.GetPartsCost(startDate, endDate).ToList();

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

        public List<facturas> GetFacturasByVehicleAndDate(string VehPlaca, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = new rvfleetEntities())
                {
                    var result = context.facturas
                        .Where(x => x.detallefactura.Any(y => y.DetPlacaVehiculo.Equals(VehPlaca)) && x.FacFechaFactura >= startDate && x.FacFechaFactura <= endDate)
                        .Include(x => x.proveedor).Include(x => x.detallefactura)
                        .ToList();

                    return result;
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