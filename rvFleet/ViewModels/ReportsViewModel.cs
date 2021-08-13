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
        public ReportsViewModel()
        { }

        public List<vehiclecosts> GetVehicleCosts()
        {
            try
            {
                List<vehiclecosts> vehicleCosts = new List<vehiclecosts>();

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
        }

        public List<recommendedmaintenance> GetRecommendedMaintenance()
        {
            try
            {
                List<recommendedmaintenance> recommendedmaintenances = new List<recommendedmaintenance>();

                using (var context = new rvfleetEntities())
                {
                    recommendedmaintenances = context.recommendedmaintenance.ToList();
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
                using (var context = new rvfleetEntities())
                {
                    var data = context.spGetRubrosNoFacturadosVehiculo().ToList();

                    foreach (var item in data)
                    {
                        model.Add(item);
                    }
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