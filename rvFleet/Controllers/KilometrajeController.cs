using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.App_Code;
using rvFleet.Models;
using rvFleet.ViewModels;

namespace rvFleet.Controllers
{
    public class KilometrajeController : Controller
    {
        KilometrajeHistoricoViewModel KilometrajeViewModel;
        VehiclesViewModel VehiclesViewModel;
        public KilometrajeController()
        {
            KilometrajeViewModel = new KilometrajeHistoricoViewModel();
            VehiclesViewModel = new VehiclesViewModel();
        }

        // GET: Kilometraje
        public ActionResult Upload(string KilKilometraje, HttpPostedFileBase KilometrajeImg)
        {
            try
            {
                kilometrajehistorico Kilometraje = new kilometrajehistorico();
                var user = BaseViewModel.GetUserData();
                var vehicle = VehiclesViewModel.GetVehiculoUsuario(user.IdUsuario);
                Kilometraje.KilUsuarioIngreso = user.IdUsuario;
                Kilometraje.KilFechaIngreso = DateTime.Today;
                Kilometraje.KilCodigoVehiculo = vehicle.VehCodigoVehiculo;
                Kilometraje.KilKilometraje = Convert.ToInt32(KilKilometraje.Replace(",", ""));
                Kilometraje.KilFotografia = this.SaveImageKilometraje(Kilometraje.KilCodigoVehiculo, KilometrajeImg);

                KilometrajeViewModel.UploadKilometraje(Kilometraje);

                return RedirectToAction("Index", "Home");
            } 
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public string SaveImageKilometraje(int CodigoVehiculo, HttpPostedFileBase Image)
        {
            string VehiculosPath = Constants.VehiculosPath;
            string filePath = Server.MapPath($"~{VehiculosPath}/{CodigoVehiculo}/Kilometrajes");
            string ext = Path.GetExtension(Image.FileName);
            string _FileName = $"Kilometraje_{CodigoVehiculo}_{DateTime.Now.ToString("yyyyMMdd")}{ext}";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string _path = Path.Combine(filePath, _FileName);
            Image.SaveAs(_path);

            return $"{VehiculosPath}/{CodigoVehiculo}/Kilometrajes/{_FileName}";
        }
    }
}
