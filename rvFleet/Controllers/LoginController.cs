using rvFleet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.ViewModels;
using System.Web.Security;
using Newtonsoft.Json;

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
        public ActionResult Index(usuario user)
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
                    CreateCookie(user);
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

        public void CreateCookie(usuario user)
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