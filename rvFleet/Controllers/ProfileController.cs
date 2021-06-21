using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.Models;
using rvFleet.ViewModels;

namespace rvFleet.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        UsuariosViewModel UsuariosViewModel;
        public ProfileController()
        {
            UsuariosViewModel = new UsuariosViewModel();
        }

        // GET: Profile
        public ActionResult Index()
        {
            try
            {
                vehiculos vehiculo = new vehiculos();
                var user = BaseViewModel.GetUserData();
                var model = UsuariosViewModel.GetUsuario(user.IdUsuario);
                var UserHasVehicle = new VehiclesViewModel().UserHasVehicle(user.IdUsuario);
                ViewBag.IdEmpresa = new SelectList(new EnterpriseViewModel().GetEmpresas(), "IdEmpresa", "NombreEmpresa", user.IdEmpresa);
                ViewBag.UserHasVehicle = UserHasVehicle;

                if (UserHasVehicle)
                {
                    vehiculo = new VehiclesViewModel().GetVehiculoUsuario(user.IdUsuario);
                }

                ViewBag.Vehiculo = vehiculo;

                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Index(usuario usuario)
        {
            try
            {
                vehiculos vehiculo = new vehiculos();
                var user = BaseViewModel.GetUserData();
                var UserHasVehicle = new VehiclesViewModel().UserHasVehicle(user.IdUsuario);
                ViewBag.IdEmpresa = new SelectList(new EnterpriseViewModel().GetEmpresas(), "IdEmpresa", "NombreEmpresa", user.IdEmpresa);
                ViewBag.UserHasVehicle = UserHasVehicle;

                if (!ModelState.IsValid)
                {
                    ViewBag.Vehiculo = vehiculo;

                    return View();
                }

                if (UserHasVehicle)
                {
                    vehiculo = new VehiclesViewModel().GetVehiculoUsuario(user.IdUsuario);
                }

                var model = UsuariosViewModel.SaveUsuario(usuario);
                ViewBag.Vehiculo = vehiculo;
                ViewBag.Message = "Datos actualizados correctamente";

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }
    }
}
