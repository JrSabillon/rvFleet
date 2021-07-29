using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rvFleet.Models;

namespace rvFleet.POCO.Reports
{
    public class ReportsModel
    {
        public ReportsModel()
        {
            VehicleCosts = new List<vehiclecosts>();
            RecommendedMaintenances = new List<recommendedmaintenance>();
            VehicleAnualCostGraphData = new List<GetVehicleAnualCostsGraphData_Result>();
            KilometrajesPorVehiculoAnoActual = new List<kilometrajeporvehiculoanoactual>();
        }

        public List<vehiclecosts> VehicleCosts { get; set; }
        public List<recommendedmaintenance> RecommendedMaintenances { get; set; }
        public List<GetVehicleAnualCostsGraphData_Result> VehicleAnualCostGraphData { get; set; }
        public List<kilometrajeporvehiculoanoactual> KilometrajesPorVehiculoAnoActual { get; set; } 
    }
}