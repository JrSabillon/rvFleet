using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.ViewModels;
using rvFleet.Models;
using Newtonsoft.Json;

namespace rvFleet.Controllers
{
    [Authorize]
    public class GraphsController : Controller
    {
        VehiclesViewModel VehiclesViewModel;
        public GraphsController()
        {
            VehiclesViewModel = new VehiclesViewModel();
        }

        public ActionResult Vehicles(string VehPlaca)
        {
            try
            {
                var vehiculos = VehiclesViewModel.GetVehiculos();
                ViewBag.VehPlaca = new SelectList(vehiculos, "VehPlaca", "VehPlaca", VehPlaca ?? string.Empty);
            
                List<GetVehicleGraphData_Result> data = VehiclesViewModel.GetGraphData(VehPlaca ?? vehiculos.FirstOrDefault().VehPlaca);
                ViewBag.JsonData = JsonConvert.SerializeObject(data);

                return View();
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }
    }
}