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
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Vehicles()
        {
            try
            {
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