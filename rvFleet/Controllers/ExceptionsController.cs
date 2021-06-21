using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rvFleet.Controllers
{
    public class ExceptionsController : Controller
    {
        // GET: Exceptions
        public ActionResult Index()
        {
            var error = Server.GetLastError();

            if(error != null)
            {
                ViewBag.Message = error.Message;
            }

            return View();
        }
    }
}