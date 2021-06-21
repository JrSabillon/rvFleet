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
        public ActionResult Index(string searchString, int? page, string type = "1")
        {
            try
            {
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
                
                //Cargar detalles de cada factura
                foreach (var item in model)
                {
                    List<detallefactura> detallefacturas = viewModel.GetDetallefacturas(item.FacCodigoOrden);
                    List<string> OrderElements = new List<string>();


                    for (int i = 0; i < detallefacturas.Count; i++)
                    {
                        OrderElements.Add($"{detallefacturas[i].DetCantidad} {detallefacturas[i].DetDescripcion}");
                    }

                    item.Detalles = string.Join(", ", OrderElements);
                    item.NombreProveedor = viewModel.GetProveedor(item.FacCodigoProveedor).ProNombre;
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    ViewBag.searchString = searchString;
                    model = model.Where(x => x.NombreProveedor.ToLower().Contains(searchString.ToLower()) || x.Detalles.ToLower().Contains(searchString.ToLower())).ToList();
                }

                ViewBag.Type = new SelectList(GetTypes(), "Value", "Text", type);
                int pageSize = 5;
                int pageNumber = (page ?? 1);
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
                new ItemModel { Text = "Òrdenes", Value = "3" },
            };

            return Types.ToList();
        }

        [HttpGet]
        public ActionResult New()
        {
            try
            {
                ViewBag.FacCodigoProveedor = new SelectList(new ProvidersViewModel().GetProveedors(), "ProCodigoProveedor", "ProNombre");
                ViewBag.DetPlacaVehiculo = new SelectList(new VehiclesViewModel().GetVehiculos(), "VehCodigoVehiculo", "VehPlaca");
                var rubros = new RubrosViewModel().GetRubros();
                ViewBag.DetCodigoRubro = new SelectList(rubros, "CodigoRubro", "NombreRubro");
                ViewBag.DetCodigoDescripcion = new SelectList(new DetalleRubroViewModel().GetRubrodetalles(rubros.FirstOrDefault().CodigoRubro), "CodigoDetalle", "NombreDetalle");

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
                            ViewBag.Message = $"Orden/Factura ingresada exitósamente. <a href='{Url.Action("New", "Orders")}' class='alert-link'>Ingresar nueva Orden/Factura</a>";
                            break;
                        default:
                            ViewBag.Message = "Modificaciones realizadas exitósamente.";
                            break;
                    }
                }

                var model = viewModel.GetFactura(FacCodigoOrden);

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
        public ActionResult Details(int FacCodigoOrden)
        {
            try
            {
                var model = viewModel.GetFactura(FacCodigoOrden);

                ViewBag.ArchivosFactura = model.archivofactura;
                ViewBag.FacCodigoProveedor = new SelectList(new ProvidersViewModel().GetProveedors(), "ProCodigoProveedor", "ProNombre", model.FacCodigoProveedor);

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
                    throw new Exception("Los datos ingresados no son correctos, verifiquelos y vuelva a intentarlo.");
                }

                facturas.FacCodigoUsuarioIngreso = BaseViewModel.GetUserData().IdUsuario;
                facturas.FacValorFactura = facturas.detallefactura.Sum(x => x.DetValor);
                
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
    }
}