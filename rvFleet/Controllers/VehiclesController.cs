using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rvFleet.ViewModels;
using rvFleet.Models;
using PagedList;
using rvFleet.App_Code;
using System.IO;

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
        public ActionResult Control(/*int? page, string searchString*/)
        {
            try
            {
                var model = VehiclesViewModel.GetVehiclesCompleteData();

                //if (!string.IsNullOrEmpty(searchString))
                //{
                //    ViewBag.searchString = searchString;
                //    model = model.Where(x => x.VehPlaca.ToUpper().Contains(searchString.ToUpper()) || x.NombreUsuario.ToUpper().Contains(searchString.ToUpper())).ToList();
                //}

                //int pageSize = 25;
                //int pageNumber = (page ?? 1);
                return View(model);
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

                //Salvar Imagenes
                vehicle.VehUrlFotoRevision = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgRevision, "Revision");
                vehicle.VehFotoFrontal = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgDelantera, "Frontal");
                vehicle.VehFotoLateralDerecha = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgDerecha, "Derecha");
                vehicle.VehFotoLateralIzquierda = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgIzquierda, "Izquierda");
                vehicle.VehFotoTrasera = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgTrasera, "Trasera");
                vehicle.VehFotoMotor = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgMotor, "Motor");
                vehicle.VehFotoInterior = this.SaveImage(vehicle.VehCodigoVehiculo, vehicle.ImgInterior, "Interior");

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

        public string SaveImage(int CodigoVehiculo, HttpPostedFileBase Image, string TipoImagen)
        {
            if(Image != null)
            {
                string VehiculosPath = Constants.VehiculosPath;
                string filePath = Server.MapPath($"~{VehiculosPath}/{CodigoVehiculo}/Imagenes");
                string ext = Path.GetExtension(Image.FileName);
                string _FileName = $"{TipoImagen}_{CodigoVehiculo}_{DateTime.Now.ToString("yyyyMMdd")}{ext}";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string _path = Path.Combine(filePath, _FileName);
                Image.SaveAs(_path);

                return $"{VehiculosPath}/{CodigoVehiculo}/Imagenes/{_FileName}";
            }

            return null;
        }

        public ActionResult Details(string VehPlaca)
        {
            throw new NotImplementedException();
        }

        public ActionResult Logs(string VehPlaca, string Option = "Comentarios")
        {
            try
            {
                LogsViewModel logsViewModel = new LogsViewModel();
                var canSeeAllVehics = logsViewModel.UserCanSeeLogs(BaseViewModel.GetUserData().IdUsuario);

                if (string.IsNullOrEmpty(VehPlaca) && !canSeeAllVehics)
                    return RedirectToAction("Index", "Home");

                var vehicles = VehiclesViewModel.GetVehiculos().ToList();
                ViewBag.SelectedPlaca = VehPlaca;
                ViewBag.VehPlaca = new SelectList(vehicles, "VehPlaca", "VehPlaca", VehPlaca);
                ViewBag.CanChange = canSeeAllVehics;
                ViewBag.Option = new SelectList(new string[] { "Comentarios", "Inspecciones" }, Option);
                ViewBag.SelectedOption = Option;

                if (Option.Equals("Comentarios"))
                {
                    //VehPlaca = string.IsNullOrEmpty(VehPlaca) ? vehicles.FirstOrDefault().VehPlaca : VehPlaca;
                    var logTypes = logsViewModel.getTipoBitacoras();
                    ViewBag.bitacoraTipo = new SelectList(logTypes, "IdOpcionRecurso", "NombreOpcionRecurso");
                    ViewBag.ComentariosModel = logsViewModel.GetBitacoravehiculos(VehPlaca).OrderByDescending(x => x.bitacoraId).ToList();
                }
                else
                {
                    ViewBag.InspectionModel = VehiclesViewModel.GetRespuestasGrouped(VehPlaca).OrderByDescending(x => x.Fecha).ToList();
                    ViewBag.Preguntas = VehiclesViewModel.GetPreguntas();
                }

                return View();
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult AddLog(bitacoravehiculo data)
        {
            try
            {
                data.bitacoraUsuario = BaseViewModel.GetUserData().IdUsuario;
                data.bitacoraFecha = DateTime.Now;
                new LogsViewModel().AddLog(data);

                return RedirectToAction("Logs", new { VehPlaca = data.bitacoraPlaca });
            }
            catch(Exception exc) 
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult AddLogComment(bitacoracomentario data, string VehPlaca)
        {
            try
            {
                data.bitacoraComUsuario = BaseViewModel.GetUserData().IdUsuario;
                data.bitacoraComFecha = DateTime.Now;
                new LogsViewModel().AddLogComment(data);

                return RedirectToAction("Logs", new { VehPlaca });
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult Inspection(string VehCodigo)
        {
            try
            {
                if (string.IsNullOrEmpty(VehCodigo))
                    return RedirectToAction("Index", "Home");

                var model = VehiclesViewModel.GetPreguntas();
                var vehicle = VehiclesViewModel.GetVehiculoById(Convert.ToInt32(VehCodigo));
                ViewBag.CodigoVehiculo = VehCodigo;
                ViewBag.VehPlaca = vehicle.VehPlaca;
                ViewBag.KilometrajeActual = vehicle.VehKilometraje ?? 0;

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult Inspections()
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