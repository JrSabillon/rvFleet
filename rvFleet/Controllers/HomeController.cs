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
            return View();
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