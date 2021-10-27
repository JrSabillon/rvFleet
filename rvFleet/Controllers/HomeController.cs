using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.ViewModels;

namespace rvFleet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult CreateHash(string data)
        {
            ViewBag.Hash = BaseViewModel.CreateHash(data);
            return View("Index");
        }
        
        public ActionResult Index()
        {
            try
            {
                VehiclesViewModel vehiclesViewModel = new VehiclesViewModel();
                KilometrajeHistoricoViewModel kilometrajeHistoricoViewModel = new KilometrajeHistoricoViewModel();

                var userData = BaseViewModel.GetUserData();
                var model = new NavViewModel().GetPrivilegios(userData.IdUsuario);
                ViewBag.UserName = userData.NombreUsuario;
                
                ///Obtener el vehiculo principal que tiene el usuario(se maneja asi para no cambiar demasiado la funcionalidad del sistema).
                var vehicle = vehiclesViewModel.GetVehiculoUsuario(userData.IdUsuario);

                if(vehicle != null)
                {
                    //ViewBag.KilometrajeUploaded = true;
                    ViewBag.HasVehicle = true;
                    ViewBag.VehPlaca = vehicle.VehPlaca;
                    ViewBag.LogsCount = new LogsViewModel().GetBitacoravehiculos(vehicle.VehPlaca).Count;
                    ViewBag.RubrosPrincipalesModel = new VehiclesViewModel().GetGraphData(vehicle.VehPlaca)
                            .Where(x => x.CodigoRubro == 42 || x.CodigoRubro == 35 || x.CodigoRubro == 36 || x.CodigoRubro == 10).ToList();
                }
                else
                {
                    ViewBag.HasVehicle = false;
                }

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult InspectionResults(string VehPlaca)
        {
            var model = new VehiclesViewModel().GetGraphData(VehPlaca)
                .Where(x => x.CodigoRubro == 42 || x.CodigoRubro == 35 || x.CodigoRubro == 36 || x.CodigoRubro == 10).ToList();
            ViewBag.VehPlaca = VehPlaca;

            return View(model);
        }
    }
}