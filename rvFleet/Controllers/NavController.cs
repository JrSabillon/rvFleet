using rvFleet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace rvFleet.Controllers
{
    public class NavController : Controller
    {
        // GET: Nav
        public ActionResult SideBar()
        {
            string UserId = BaseViewModel.GetUserData().IdUsuario;
            var model = new NavViewModel().GetPrivilegios(UserId);
            return PartialView("_SideBar", model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}