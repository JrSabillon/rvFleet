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
            var userData = BaseViewModel.GetUserData();
            var model = new NavViewModel().GetPrivilegios(userData.IdUsuario);
            ViewBag.UserName = userData.NombreUsuario;

            return View(model);
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