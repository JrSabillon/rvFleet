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

                var vehicle = vehiclesViewModel.GetVehiculoUsuario(userData.IdUsuario);
                if(vehicle != null)
                {
                    //el usuario tiene un vehiculo asignado, verificar que ya ingreso el kilometraje de hoy.
                    ViewBag.KilometrajeUploaded = kilometrajeHistoricoViewModel.KilometrajeUploaded(vehicle.VehCodigoVehiculo);
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
    }
}