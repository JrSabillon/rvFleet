using rvFleet.Models;
using rvFleet.POCO.Reports;
using rvFleet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rvFleet.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        ReportsViewModel ReportsViewModel;
        public ReportsController()
        {
            ReportsViewModel = new ReportsViewModel();
        }

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Vehicles()
        {
            try
            {
                var model = new ReportsModel();
                int CarsAssigned, CarsUnassigned, TotalCars;
                ReportsViewModel.CarInventory(out CarsAssigned, out CarsUnassigned, out TotalCars);

                ViewBag.CarsAssigned = CarsAssigned;
                ViewBag.CarsUnassigned = CarsUnassigned;
                ViewBag.TotalCars = TotalCars;
                model.VehicleCosts = ReportsViewModel.GetVehicleCosts();
                model.RecommendedMaintenances = ReportsViewModel.GetRecommendedMaintenance();
                model.VehicleAnualCostGraphData = ReportsViewModel.VehicleAnualCosts(DateTime.Now.Year);
                model.KilometrajesPorVehiculoAnoActual = ReportsViewModel.GetKilometrajePorVehiculoAnoActual();

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult VehiclesCosts(string startDate, string endDate)
        {
            try
            {
                if(string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetVehicleCosts_Filtered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult PartsCost(string startDate, string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetPartsCost_Filtered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult OrdersDetails(string VehPlaca, string startDate, string endDate)
        {
            ViewBag.VehPlaca = VehPlaca;
            ViewBag.startDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd");
            ViewBag.endDate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd");
            var model = ReportsViewModel.GetFacturasByVehicleAndDate(VehPlaca, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            
            foreach (var item in model)
            {
                //List<detallefactura> detallefacturas = viewModel.GetDetallefacturas(model[i].FacCodigoOrden);
                List<detallefactura> detallefacturas = item.detallefactura.ToList();
                List<string> OrderElements = new List<string>();


                for (int j = 0; j < detallefacturas.Count; j++)
                {
                    OrderElements.Add($"{detallefacturas[j].DetCantidad} {detallefacturas[j].DetDescripcion}");
                }

                item.Detalles = string.Join(", ", OrderElements);
                item.NombreProveedor = item.proveedor.ProNombre;
            }

            ViewBag.Total = model.Sum(x => x.FacValorFactura);

            return View(model);
        }
    }
}