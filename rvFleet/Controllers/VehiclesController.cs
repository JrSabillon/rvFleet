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
using OfficeOpenXml;
using Newtonsoft.Json;

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
                var model = VehiclesViewModel.GetVehiculos();

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
                string _FileName = $"{TipoImagen}_{CodigoVehiculo}_{DateTime.Now:yyyyMMdd}{ext}";

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
                string UserId = BaseViewModel.GetUserData().IdUsuario;
                var canSeeAllVehics = logsViewModel.UserCanSeeLogs(UserId);

                //if (string.IsNullOrEmpty(VehPlaca))
                    //return RedirectToAction("Index", "Home");

                var vehicles = VehiclesViewModel.GetVehiculos().ToList();

                if (!canSeeAllVehics)
                    VehPlaca = vehicles.Where(x => x.VehCodigoUsuario == UserId).FirstOrDefault().VehPlaca;
                
                
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

        /// <summary>
        /// Antes funcionaba asi el checklist.
        /// </summary>
        /// <param name="VehCodigo">Codigo de vehiculo, entero.</param>
        /// <returns></returns>
        //public ActionResult Inspection(string VehCodigo)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(VehCodigo))
        //            return RedirectToAction("Index", "Home");

        //        var model = VehiclesViewModel.GetPreguntas();
        //        var vehicle = VehiclesViewModel.GetVehiculoById(Convert.ToInt32(VehCodigo));
        //        ViewBag.CodigoVehiculo = VehCodigo;
        //        ViewBag.VehPlaca = vehicle.VehPlaca;
        //        ViewBag.KilometrajeActual = vehicle.VehKilometraje ?? 0;

        //        return View(model);
        //    }
        //    catch(Exception exc)
        //    {
        //        ViewBag.Message = exc.Message;
        //        return View("Error");
        //    }
        //}
        
        public ActionResult Inspection()
        {
            try
            {
                var userData = BaseViewModel.GetUserData();
                var vehiculos = VehiclesViewModel.GetVehiculosAsignados(userData.IdUsuario);
                ViewBag.VehPlaca = new SelectList(vehiculos, "CodigoVehiculo", "VehPlaca");

                return View();
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult _Inspection(string VehCodigo)
        {
            try
            {
                var model = VehiclesViewModel.GetPreguntas();
                var vehicle = VehiclesViewModel.GetVehiculoById(Convert.ToInt32(VehCodigo));
                ViewBag.CodigoVehiculo = VehCodigo;
                ViewBag.VehPlaca = vehicle.VehPlaca;
                ViewBag.VehCodigoVehiculo = vehicle.VehCodigoVehiculo;
                ViewBag.KilometrajeActual = vehicle.VehKilometraje ?? 0;

                return PartialView("_Inspection", model);
            }
            catch (Exception exc)
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

        public ActionResult LogReview(int VehCodigo)
        {
            try
            {
                var vehicle = VehiclesViewModel.GetVehiculoById(VehCodigo);
                ViewBag.VehPlaca = vehicle.VehPlaca;
                var model = VehiclesViewModel.GetGraphData(vehicle.VehPlaca)
                                .Where(x => x.CodigoRubro == 42 || x.CodigoRubro == 35 || x.CodigoRubro == 36 || x.CodigoRubro == 10).ToList();
            
                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult VehicleQuickReview(string VehPlaca)
        {
            try
            {
                var model = VehiclesViewModel.GetGraphData(VehPlaca)
                                .Where(x => x.CodigoRubro == 42 || x.CodigoRubro == 35 || x.CodigoRubro == 36 || x.CodigoRubro == 10).ToList();
                ViewBag.VehPlaca = VehPlaca;

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public FileResult ExportVehicles(string data)
        {
            var model = JsonConvert.DeserializeObject<List<vehiclefulldata>>(data);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();

            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Vehiculos");
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1"].Value = "Placa";
            Sheet.Cells["B1"].Value = "Color";
            Sheet.Cells["C1"].Value = "Marca/Modelo";
            Sheet.Cells["D1"].Value = "Año";
            Sheet.Cells["E1"].Value = "Kilometraje";
            Sheet.Cells["F1"].Value = "Empresa";
            Sheet.Cells["G1"].Value = "Conductor";
            Sheet.Cells["H1"].Value = "Taller";

            int row = 2;
            foreach (var item in model)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.VehPlaca;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.VehColor;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.VehMarca + " " + item.VehModelo;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.VehAno;
                Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "#,##0";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.VehKilometraje;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.NombreEmpresa;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.NombreUsuario;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.ProNombre;

                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();

            return File(Ep.GetAsByteArray(), "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Vehiculos_{DateTime.Now:yyyyMMddHHmmssffff}.xlsx");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="VehPlaca">si se ejecuta desde la vista este parametro contendra el codigo del vehiculo para posteriormente cambiarse por la placa.</param>
        /// <returns></returns>
        public ActionResult MyVehicles(string VehPlaca)
        {
            try
            {
                var userData = BaseViewModel.GetUserData();
                var vehiculosAsignados = VehiclesViewModel.GetVehiculosAsignados(userData.IdUsuario);
                vehiculos vehiculo = new vehiculos();

                if (!string.IsNullOrEmpty(VehPlaca))
                {
                    vehiculo = VehiclesViewModel.GetVehiculoById(Convert.ToInt32(VehPlaca));
                }
                else
                {
                    vehiculo = VehiclesViewModel.GetVehiculoById(vehiculosAsignados.FirstOrDefault().CodigoVehiculo);
                }

                VehPlaca = vehiculo.VehPlaca;

                var graphData = VehiclesViewModel.GetGraphData(VehPlaca)
                                .Where(x => x.CodigoRubro == 42 || x.CodigoRubro == 35 || x.CodigoRubro == 36 || x.CodigoRubro == 10).ToList();

                ViewBag.GraphData = graphData;
                ViewBag.VehPlaca = new SelectList(vehiculosAsignados, "CodigoVehiculo", "VehPlaca");

                return View(vehiculo);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult UpdateVehicleKM(int VehCodigoVehiculo, int VehKilometraje)
        {
            try
            {
                VehiclesViewModel.UpdateKilometraje(VehCodigoVehiculo, VehKilometraje);

                return RedirectToAction("MyVehicles", new { VehPlaca = VehCodigoVehiculo });
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult QuickInspection(int VehCodigoVehiculo)
        {
            try
            {
                var vehicle = VehiclesViewModel.GetVehiculoById(VehCodigoVehiculo);
                ViewBag.VehCodigoVehiculo = VehCodigoVehiculo;

                return View(vehicle);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }
    }
}