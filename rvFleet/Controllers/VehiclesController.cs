using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.ViewModels;
using rvFleet.Models;
using PagedList;

namespace rvFleet.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        VehiclesViewModel VehiclesViewModel;
        public VehiclesController()
        {
            VehiclesViewModel = new VehiclesViewModel();
        }
        
        // GET: Vehicles
        public ActionResult Control(int? page, string searchString)
        {
            try
            {
                var model = VehiclesViewModel.GetVehiclesCompleteData();

                if (!string.IsNullOrEmpty(searchString))
                {
                    ViewBag.searchString = searchString;
                    model = model.Where(x => x.VehPlaca.ToUpper().Contains(searchString.ToUpper()) || x.NombreUsuario.ToUpper().Contains(searchString.ToUpper())).ToList();
                }

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(model.ToPagedList(pageNumber, pageSize));
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult Deliver()
        {
            return View();
        }

        public ActionResult Receive()
        {
            return View();
        }

        [HttpGet]
        public ActionResult New()
        {
            try
            {
                var users = new UsuariosViewModel().GetUsuarios();
                ViewBag.VehCodigoUsuario = new SelectList(users, "IdUsuario", "NombreUsuario");
                var enterprises = new EnterpriseViewModel().GetEmpresas();
                ViewBag.VehCodigoEmpresa = new SelectList(enterprises, "IdEmpresa", "NombreEmpresa");
                var providers = new ProvidersViewModel().GetProveedors();
                ViewBag.VehCodigoProveedor = new SelectList(providers, "ProCodigoProveedor", "ProNombre");

                return View();
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult New(vehiculos vehicle)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var users = new UsuariosViewModel().GetUsuarios();
                    ViewBag.VehCodigoUsuario = new SelectList(users, "IdUsuario", "NombreUsuario", vehicle.VehCodigoUsuario);
                    var enterprises = new EnterpriseViewModel().GetEmpresas();
                    ViewBag.VehCodigoEmpresa = new SelectList(enterprises, "IdEmpresa", "NombreEmpresa", vehicle.VehCodigoEmpresa);
                    var providers = new ProvidersViewModel().GetProveedors();
                    ViewBag.VehCodigoProveedor = new SelectList(providers, "ProCodigoProveedor", "ProNombre", vehicle.VehCodigoProveedor);

                    return View();
                }

                var newVehicle = VehiclesViewModel.CreateVehicle(vehicle);
                Session["CarCreated"] = true;
                return RedirectToAction("Edit", new { newVehicle.VehPlaca, ExecutedAction = "C_C" });
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult Edit(string VehPlaca, string ExecutedAction)
        {
            try
            {
                if (!string.IsNullOrEmpty(ExecutedAction))
                {
                    switch (ExecutedAction)
                    {
                        case "C_C":
                            ViewBag.Message = "Vehículo registrado exitósamente";
                            break;
                    }
                }

                var model = VehiclesViewModel.GetVehiculo(VehPlaca);
                var users = new UsuariosViewModel().GetUsuarios();
                ViewBag.VehCodigoUsuario = new SelectList(users, "IdUsuario", "NombreUsuario", model.VehCodigoUsuario);
                var enterprises = new EnterpriseViewModel().GetEmpresas();
                ViewBag.VehCodigoEmpresa = new SelectList(enterprises, "IdEmpresa", "NombreEmpresa", model.VehCodigoEmpresa);
                var providers = new ProvidersViewModel().GetProveedors();
                ViewBag.VehCodigoProveedor = new SelectList(providers, "ProCodigoProveedor", "ProNombre", model.VehCodigoProveedor);

                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Edit(vehiculos vehicle)
        {
            try
            {
                var users = new UsuariosViewModel().GetUsuarios();
                ViewBag.VehCodigoUsuario = new SelectList(users, "IdUsuario", "NombreUsuario", vehicle.VehCodigoUsuario);
                var enterprises = new EnterpriseViewModel().GetEmpresas();
                ViewBag.VehCodigoEmpresa = new SelectList(enterprises, "IdEmpresa", "NombreEmpresa", vehicle.VehCodigoEmpresa);
                var providers = new ProvidersViewModel().GetProveedors();
                ViewBag.VehCodigoProveedor = new SelectList(providers, "ProCodigoProveedor", "ProNombre", vehicle.VehCodigoProveedor);

                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }

                var model = VehiclesViewModel.UpdateVehicle(vehicle);

                ViewBag.Message = "Modificaciones realizadas exitósamente";
                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult Details(string VehPlaca)
        {
            throw new NotImplementedException();
        }
    }
}