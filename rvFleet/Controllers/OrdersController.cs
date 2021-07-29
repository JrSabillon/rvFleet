using rvFleet.Models;
using rvFleet.POCO;
using rvFleet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using System.IO;
using rvFleet.App_Code;
using System.Data.OleDb;
using System.Data;
using ClosedXML.Excel;

namespace rvFleet.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private OrdersViewModel viewModel;

        public OrdersController()
        {
            //Enlazar el modelado con el controlador
            viewModel = new OrdersViewModel();
        }

        // GET: Orders
        public ActionResult Index(string searchString, int? page, string VehPlaca, string sortOrder = "fecha_desc", string type = "1")
        {
            try
            {
                int pageSize = 5;
                int pageNumber = (page ?? 1);

                List<facturas> model = new List<facturas>();
                ViewBag.SelectedType = type;
                //Cargar datos de las ordenes/facturas
                switch (type)
                {
                    case "1":
                        model = viewModel.GetOrdenesFacturas();
                        break;
                    case "2":
                        model = viewModel.GetFacturas();
                        break;
                    case "3":
                        model = viewModel.GetOrdenes();
                        break;
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    ViewBag.searchString = searchString;
                    model = model.Where(x => x.proveedor.ProNombre.ToLower().Contains(searchString.ToLower()) || x.detallefactura.Any(y => y.DetDescripcion.ToLower().Contains(searchString.ToLower()))).ToList();
                }

                ViewBag.VehPlaca = new SelectList(new VehiclesViewModel().GetVehiculos(), "VehPlaca", "VehPlaca", VehPlaca);

                if (!string.IsNullOrEmpty(VehPlaca))
                {
                    ViewBag.SelectedVehPlaca = VehPlaca;
                    model = model.Where(x => x.detallefactura.Any(y => y.DetPlacaVehiculo.Equals(VehPlaca))).ToList();
                }

                switch (sortOrder)
                {
                    case "factura":
                        model = model.OrderBy(x => x.FacNumeroFactura).ToList();
                        break;
                    case "factura_desc":
                        model = model.OrderByDescending(x => x.FacNumeroFactura).ToList();
                        break;
                    case "fecha":
                        model = model.OrderBy(x => x.FacFechaOrden).ToList();
                        break;
                    case "fecha_desc":
                        model = model.OrderByDescending(x => x.FacFechaOrden).ToList();
                        break;
                    case "proveedor":
                        model = model.OrderBy(x => x.proveedor.ProNombre).ToList();
                        break;
                    case "proveedor_desc":
                        model = model.OrderByDescending(x => x.proveedor.ProNombre).ToList();
                        break;
                    case "total":
                        model = model.OrderBy(x => x.FacValorFactura).ToList();
                        break;
                    case "total_desc":
                        model = model.OrderByDescending(x => x.FacValorFactura).ToList();
                        break;
                    default:
                        model = model.OrderByDescending(x => x.FacFechaOrden).ToList();
                        break;
                }

                ViewBag.SortOrder = sortOrder; //Para saber que icono mostrar
                ViewBag.SortDate = sortOrder == "fecha" ? "fecha_desc" : "fecha";
                ViewBag.SortFacNumeroFactura = sortOrder == "factura" ? "factura_desc" : "factura";
                ViewBag.SortProveedor = sortOrder == "proveedor" ? "proveedor_desc" : "proveedor";
                //ViewBag.SortDetalle = sortOrder == "detalle" ? "detalle_desc" : "detalle";
                //ViewBag.SortVehiculos = sortOrder == "vehiculos" ? "vehiculos_desc" : "vehiculos";
                ViewBag.SortTotal = sortOrder == "total" ? "total_desc" : "total";

                ViewBag.Type = new SelectList(GetTypes(), "Value", "Text", type);
                return View(model.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        private List<ItemModel> GetTypes()
        {
            var Types = new List<ItemModel>
            {
                new ItemModel { Text = "Todas", Value = "1" },
                new ItemModel { Text = "Facturas", Value = "2" },
                new ItemModel { Text = "Órdenes", Value = "3" },
            };

            return Types.ToList();
        }

        [HttpGet]
        public ActionResult New(int? FacCodigoProveedor, string FacNumeroFactura)
        {
            try
            {
                ViewBag.FacCodigoProveedor = new SelectList(new ProvidersViewModel().GetProveedors(), "ProCodigoProveedor", "ProNombre", FacCodigoProveedor ?? 0);
                ViewBag.DetPlacaVehiculo = new SelectList(new VehiclesViewModel().GetVehiculos(), "VehCodigoVehiculo", "VehPlaca");
                var rubros = new RubrosViewModel().GetRubros();
                ViewBag.DetCodigoRubro = new SelectList(rubros, "CodigoRubro", "NombreRubro");
                ViewBag.DetCodigoDescripcion = new SelectList(new DetalleRubroViewModel().GetRubrodetalles(rubros.FirstOrDefault().CodigoRubro), "CodigoDetalle", "NombreDetalle");
                ViewBag.FacNumeroFactura = FacNumeroFactura;

                return View();
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Edit(int FacCodigoOrden, string ExecutedAction)
        {
            try
            {
                var model = viewModel.GetFactura(FacCodigoOrden);

                if (!string.IsNullOrEmpty(ExecutedAction))
                {
                    ViewBag.Updated = "Updated";
                    switch (ExecutedAction)
                    {
                        case "I_R":
                            //Image Removed
                            ViewBag.Message = "Archivo borrado exitósamente.";
                            break;
                        case "I_A":
                            //Image Added
                            ViewBag.Message = "Archivo subido exitósamente.";
                            break;
                        case "ADD":
                            //New Record
                            ViewBag.Message = $"Orden/Factura ingresada exitósamente. <a href='{Url.Action("New", "Orders", new { model.FacCodigoProveedor, model.FacNumeroFactura })}' class='alert-link'>Ingresar nueva Orden/Factura</a>";
                            break;
                        default:
                            ViewBag.Message = "Modificaciones realizadas exitósamente.";
                            break;
                    }
                }


                ViewBag.ArchivosFactura = model.archivofactura;
                ViewBag.FacCodigoProveedor = new SelectList(new ProvidersViewModel().GetProveedors(), "ProCodigoProveedor", "ProNombre", model.FacCodigoProveedor);
                ViewBag.DetPlacaVehiculo = new SelectList(new VehiclesViewModel().GetVehiculos(), "VehCodigoVehiculo", "VehPlaca");
                var rubros = new RubrosViewModel().GetRubros();
                ViewBag.DetCodigoRubro = new SelectList(rubros, "CodigoRubro", "NombreRubro");
                ViewBag.DetCodigoDescripcion = new SelectList(new DetalleRubroViewModel().GetRubrodetalles(rubros.FirstOrDefault().CodigoRubro), "CodigoDetalle", "NombreDetalle");

                foreach (var item in model.detallefactura)
                {
                    var rubro = rubros.Where(x => x.CodigoRubro.Equals(Convert.ToInt32(item.DetCodigoRubro))).FirstOrDefault();
                    item.NombreRubro = rubro.NombreRubro;
                }

                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public JsonResult Edit(facturas facturas)
        {
            try
            {
                int identity = 0;
                foreach (var item in facturas.detallefactura)
                {
                    identity++;
                    item.DetCodigoDetalle = identity;
                }
                var factura = viewModel.UpdateFactura(facturas);

                return Json(new { status = HttpStatusCode.OK, message = Url.Action("Edit", "Orders", new { factura.FacCodigoOrden, ExecutedAction = "UPD" }) });
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return Json(new { status = HttpStatusCode.InternalServerError, message = exc.Message });
            }
        }

        [HttpGet]
        public ActionResult Details(int FacCodigoOrden, string VehPlaca, string startDate, string endDate, string source)
        {
            try
            {
                if (source == "Reportes")
                {
                    ViewBag.Source = source;
                    ViewBag.VehPlaca = VehPlaca;
                    ViewBag.startDate = startDate;
                    ViewBag.endDate = endDate;
                }

                var model = viewModel.GetFactura(FacCodigoOrden);

                ViewBag.ArchivosFactura = model.archivofactura;
                ViewBag.FacCodigoProveedor = new SelectList(new ProvidersViewModel().GetProveedors(), "ProCodigoProveedor", "ProNombre", model.FacCodigoProveedor);
                var rubros = new RubrosViewModel().GetRubros();

                foreach (var item in model.detallefactura)
                {
                    var rubro = rubros.Where(x => x.CodigoRubro.Equals(Convert.ToInt32(item.DetCodigoRubro))).FirstOrDefault();
                    item.NombreRubro = rubro.NombreRubro;
                }

                return View(model);
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public JsonResult GetRubroDetalles(int CodigoRubro)
        {
            try
            {
                var rubros = new DetalleRubroViewModel().GetRubrodetalles(CodigoRubro).ToList();
                foreach(var rubro in rubros)
                {
                    rubro.rubros = new rubros();
                }
                return Json(rubros, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exc)
            {
                return Json(new { status = HttpStatusCode.InternalServerError, message = exc.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult New(facturas facturas)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Los datos ingresados no son correctos, verifíquelos y vuelva a intentarlo.");
                }

                facturas.FacCodigoUsuarioIngreso = BaseViewModel.GetUserData().IdUsuario;
                var total = facturas.detallefactura.Sum(x => x.DetValor);
                facturas.FacValorFactura = facturas.FacAplicaImpuesto.Value ? total * 0.15 + total : total;
                
                int identity = 0;
                foreach (var item in facturas.detallefactura)
                {
                    identity++;
                    item.DetCodigoDetalle = identity;
                }

                viewModel.CreateFactura(facturas);

                return Json(new { status = HttpStatusCode.OK, message = Url.Action("Edit", "Orders", new { facturas.FacCodigoOrden, ExecutedAction = "ADD" })});
            }
            catch(Exception exc)
            {
                return Json(new { status = HttpStatusCode.InternalServerError, message = exc.Message });
            }
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase image, int FacCodigoOrden)
        {
            try
            {
                string FacturasPath = Constants.FacturasPath; 
                string filePath = Server.MapPath($"~{FacturasPath}/{FacCodigoOrden}");

                if(!Directory.Exists(filePath))
                {

                    Directory.CreateDirectory(filePath);
                }

                int FacArchivoCodigo = new ArchivoFacturaViewModel().GetNextId(FacCodigoOrden);
                string ext = Path.GetExtension(image.FileName);
                string _FileName = $"O-F_{FacCodigoOrden}_{FacArchivoCodigo}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext}";

                archivofactura archivofactura = new archivofactura()
                {
                    FacArchivoCodigo = FacArchivoCodigo,
                    FacArchivoUsuario = BaseViewModel.GetUserData().IdUsuario,
                    FacArchivoFechaSubida = DateTime.Now,
                    FacCodigoOrden = FacCodigoOrden,
                    FacArchivoNombre = _FileName,
                    FacArchivoUrl = $"{FacturasPath}/{FacCodigoOrden}/{_FileName}"
                };

                string _path = Path.Combine(filePath, _FileName);
                image.SaveAs(_path);

                new ArchivoFacturaViewModel().SaveFile(archivofactura);
                return RedirectToAction("Edit", new { FacCodigoOrden, ExecutedAction = "I_A" });
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult RemoveFile(int FacCodigoOrden, int FacArchivoCodigo)
        {
            try
            {
                string FacturasPath = Constants.FacturasPath;
                var archivoFactura = new ArchivoFacturaViewModel().RemoveArchivoFacura(FacCodigoOrden, FacArchivoCodigo);
                string file = Path.Combine(Server.MapPath($"{FacturasPath}/{FacCodigoOrden}"), archivoFactura.FacArchivoNombre);
                
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                return RedirectToAction("Edit", new { FacCodigoOrden, ExecutedAction = "I_R" });
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public JsonResult GetNumeroFactura(int CodigoProveedor)
        {
            try
            {
                string NumeroFactura = viewModel.GetNumeroFactura(CodigoProveedor);

                return Json(new { status = HttpStatusCode.OK, message = NumeroFactura }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exc)
            {
                return Json(new { status = HttpStatusCode.InternalServerError, message = exc.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Historic()
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

        public ActionResult ReadExcel(HttpPostedFileBase excelFile)
        {
            try
            {
                string fileName = "TempFacts.xlsx";
                string path = Server.MapPath("~/TempFiles/" + fileName);
                excelFile.SaveAs(path);

                var workbook = new XLWorkbook(path);
                var ws1 = workbook.Worksheet(1);

                var rowsCount = ws1.Rows().Count();
                //List<FacturaHistorico> historicos = new List<FacturaHistorico>();
                List<facturas> facturas = new List<facturas>();

                int Count = 0;
                var vehiculos = new VehiclesViewModel().GetVehiculos();
                ///empezamos a recorrer el arreglo desde la posicion 2 para no contar el header.
                for (int i = 2; i <= rowsCount; i++)
                {
                    Count++;
                    var row = ws1.Row(i);

                    if (!row.IsEmpty())
                    {
                        try
                        { 
                            //if(Count == 146)
                            //{
                            //    //var x = 146;
                            //}

                            facturas factura = new facturas
                            {
                                FacFechaFactura = !string.IsNullOrEmpty(row.Cell(2).Value.ToString()) ? DateTime.Parse(row.Cell(2).Value.ToString()) : DateTime.Now,
                                FacCodigoProveedor = Convert.ToInt32(row.Cell(14).Value.ToString()),
                                FacNumeroFactura = row.Cell(5).Value.ToString(),
                                FacCodigoDetalleFactura = 0,
                                FacValorFactura = Convert.ToDouble(row.Cell(7).Value.ToString()),
                                FacComentario = row.Cell(12).Value.ToString() + " - Historico",
                                FacFechaEntregaAdmin = null,
                                FacUrlFoto = null,
                                FacCodigoUsuarioIngreso = "w.sabillon",
                                FacFechaOrden = DateTime.Parse(row.Cell(2).Value.ToString()),
                                FacUsuarioPago = row.Cell(3).Value.ToString(),
                                FacAplicaImpuesto = false
                            };

                            factura.detallefactura.Add(new detallefactura
                            {
                                DetValor = Convert.ToDouble(row.Cell(7).Value.ToString()),
                                DetCantidad = 1,
                                DetCodigoRubro = row.Cell(13).Value.ToString(),
                                DetKilometraje = !string.IsNullOrEmpty(row.Cell(9).Value.ToString()) ? Convert.ToInt32(row.Cell(9).Value.ToString().Replace(",", "")) : 0,
                                DetPlacaVehiculo = row.Cell(8).Value.ToString(),
                                DetDescripcion = row.Cell(6).Value.ToString()
                            });

                            var vehiculo = vehiculos.Where(x => x.VehPlaca.Equals(factura.detallefactura.FirstOrDefault().DetPlacaVehiculo)).FirstOrDefault();
                            if(vehiculo != null)
                            {
                                new OrdersViewModel().CreateFactura(factura);
                            }
                            //facturas.Add(factura);
                        }
                        catch
                        {
                            throw new Exception("Error en la fila: " + Count);
                        }
                    }
                    else
                    {
                        break;
                    }
                }


                //Count = 0;
                //foreach (var item in facturas)
                //{
                //    Count++;

                //    try
                //    {
                //        var vehiculo = vehiculos.Where(x => x.VehPlaca.Equals(item.detallefactura.FirstOrDefault().DetPlacaVehiculo)).FirstOrDefault();
                //        if (vehiculo != null)
                //        {
                //            if(Count == 5)
                //            {
                //                var x = 0;
                //            }
                //            //viewModel.CreateFactura(item);
                //            kilometrajehistorico kilometrajehistorico = new kilometrajehistorico
                //            {
                //                KilCodigoVehiculo = vehiculo.VehCodigoVehiculo,
                //                KilFechaIngreso = item.FacFechaFactura.Value,
                //                KilKilometraje = item.detallefactura.FirstOrDefault().DetKilometraje,
                //                KilComentario = "Agregado automatico",
                //                KilUsuarioIngreso = "w.sabillon",
                //                KilFotografia = string.Empty
                //            };


                //            new KilometrajeHistoricoViewModel().UploadKilometraje(kilometrajehistorico);
                //        }
                //    }
                //    catch
                //    {
                //        throw new Exception("Error al ingresar el registro: " + Count);
                //    }
                //}

                return RedirectToAction("Historic");
            }
            catch(Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public JsonResult ValidateNumeroFactura(string FacNumeroFactura)
        {
            try
            {
                return viewModel.NumeroFacturaDisponible(FacNumeroFactura) ? 
                    Json(new { status = HttpStatusCode.OK, message = "Ok" }, JsonRequestBehavior.AllowGet) : 
                    Json(new { status = HttpStatusCode.OK, message = "Error" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exc)
            {
                return Json(new { status = HttpStatusCode.Conflict, message = exc.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}