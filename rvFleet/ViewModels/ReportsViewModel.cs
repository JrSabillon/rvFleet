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
        MySqlConnection sqlConnection;
        public ReportsViewModel()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["rvFleetStaticConnection"].ConnectionString;
            sqlConnection = new MySqlConnection(connectionString);
        }

        public List<vehiclecosts> GetVehicleCosts()
        {
            try
            {
                List<vehiclecosts> vehicleCosts = new List<vehiclecosts>();

                //string query = "SELECT A.DETPLACAVEHICULO, SUM(A.DETVALOR) VALOR FROM DETALLEFACTURA A INNER JOIN FACTURAS B ON A.DETCODIGOORDEN = B.FACCODIGOORDEN " +
                //    "WHERE YEAR(FacFechaFactura) = YEAR(NOW()) AND MONTH(facfechafactura) = MONTH(NOW()) " +
                //    "GROUP BY A.DETPLACAVEHICULO " +
                //    "ORDER BY VALOR DESC;";
                //MySqlCommand cmd = new MySqlCommand(query, sqlConnection);
                //sqlConnection.Open();

                //using (MySqlDataReader reader = cmd.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                //        vehicleCosts.Add(new VehicleCosts
                //        {
                //            VehPlaca = reader.GetString(0),
                //            VehValor = reader.GetInt32(1)
                //        });
                //    }
                //}

                using (var context = new rvfleetEntities())
                {
                    vehicleCosts = context.vehiclecosts.ToList();
                }

                return vehicleCosts;
            }
            catch(MySqlException dbExc)
            {
                throw new ApplicationException($"{Constants.DB_Error} - {dbExc.Message}");
            }
            catch(Exception exc)
            {
                throw new ApplicationException($"{Constants.App_Error} - {exc.Message}");
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public List<recommendedmaintenance> GetRecommendedMaintenance()
        {
            try
            {
                //List<RecommendenMaintenance> recommendenMaintenances = new List<RecommendenMaintenance>();

                //string query = "SELECT a.DetPlacaVehiculo, d.NombreRubro, d.DistanciaCambio - (c.VehKilometraje - a.DetKilometraje) VidaRestante FROM detallefactura a INNER JOIN facturas b " +
                //    "ON a.detcodigoorden = b.faccodigoorden " +
                //    "INNER JOIN vehiculos c " +
                //    "ON a.detplacavehiculo = c.VehPlaca " +
                //    "INNER JOIN rubros d " +
                //    "ON a.DetCodigoRubro = d.CodigoRubro " +
                //    "WHERE DETPLACAVEHICULO = VehPlaca AND ROUND(100 - ((IFNULL(c.VehKilometraje, A.DetKilometraje) - a.DetKilometraje) / d.DistanciaCambio * 100), 2) < 30 " +
                //    "GROUP BY detcodigorubro " +
                //    "ORDER BY detcodigorubro, DetKilometraje Desc; ";

                //MySqlCommand cmd = new MySqlCommand(query, sqlConnection);
                //sqlConnection.Open();

                //using (MySqlDataReader reader = cmd.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                //        recommendenMaintenances.Add(new RecommendenMaintenance
                //        {
                //            VehPlaca = reader.GetString(0),
                //            NombreRubro = reader.GetString(1),
                //            VidaRestante = reader.GetInt32(2)
                //        });
                //    }
                //}

                List<recommendedmaintenance> recommendedmaintenances = new List<recommendedmaintenance>();

                using (var context = new rvfleetEntities())
                {
                    recommendedmaintenances = context.recommendedmaintenance.ToList();
                }

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
            finally
            {
                sqlConnection.Close();
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
            finally
            {
                sqlConnection.Close();
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
            finally
            {
                sqlConnection.Close();
            }
        }

        public List<kilometrajeporvehiculoanoactual> GetKilometrajePorVehiculoAnoActual()
        {
            try
            {
                List<kilometrajeporvehiculoanoactual> data = new List<kilometrajeporvehiculoanoactual>();

                using (var context = new rvfleetEntities())
                {
                    data = context.kilometrajeporvehiculoanoactual.ToList();
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
            finally
            {
                sqlConnection.Close();
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