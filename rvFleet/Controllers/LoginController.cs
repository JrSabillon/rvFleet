using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.ViewModels;
using System.Web.Security;
using Newtonsoft.Json;
using rvFleet.POCO;

namespace rvFleet.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                bool IsValidated; string Message;
                new LoginViewModel().LoginUser(user.IdUsuario, BaseViewModel.CreateHash(user.Contrasena), out IsValidated, out Message, user);

                if(IsValidated)
                {

                    ///Funcionalidad NUEVA: un usuario puede tener varios vehiculos, con esto se verificara el checklist ahora.
                    var vehicles = new VehiclesViewModel().GetVehiculosAsignados(user.IdUsuario);
                    ///mas que 1 quiere decir que son multiples vehiculos bajo su control.
                    user.MultipleVehicles = vehicles.Count > 1;
                    CreateCookie(user);

                    if(vehicles.Count > 0)
                    {
                        //el usuario tiene uno o mas vehiculos asignados, verificar que ya ingreso el kilometraje de hoy.
                        //var DailyCheckCompleted = kilometrajeHistoricoViewModel.KilometrajeUploaded(vehicle.VehCodigoVehiculo);
                        var DailyCheckCompleted = vehicles.Where(x => x.VehPlaca.Contains("PENDIENTE")).Count();

                        if (DailyCheckCompleted > 0)
                        {
                            //return RedirectToAction("Inspection", "Vehicles", new { VehCodigo = vehicle.VehCodigoVehiculo });
                            return RedirectToAction("Inspection", "Vehicles");
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Message = Message;
                return View();
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View();
            }
        }

        public void CreateCookie(LoginModel user)
        {
            FormsAuthenticationTicket ticket;
            string cookieStr;
            HttpCookie cookie;
            ticket = new FormsAuthenticationTicket(1, user.IdUsuario, DateTime.Now, DateTime.Now.AddMinutes(30), false, JsonConvert.SerializeObject(user));
            cookieStr = FormsAuthentication.Encrypt(ticket);
            cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieStr);
            cookie.Path = FormsAuthentication.FormsCookiePath;
            Response.Cookies.Add(cookie);
        }
    }
}