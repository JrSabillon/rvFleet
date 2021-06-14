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
        public ActionResult Index(string searchString, string type = "1")
        {
            try
            {
                List<facturas> model = new List<facturas>();
                ViewBag.SelectedType = type;
                //Cargar datos de las ordenes/facturas
                switch (type)
                {
                    case "1":
                        model = viewModel.GetOrdenes();
                        break;
                    case "2":
                        model = viewModel.GetFacturas();
                        break;
                    case "3":
                        model = viewModel.GetOrdenesFacturas();
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
                return View(model);
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
                new ItemModel { Text = "Órdenes", Value = "1" },
                new ItemModel { Text = "Facturas", Value = "2" },
                new ItemModel { Text = "Todas", Value = "3" },
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

                return Json(new { status = HttpStatusCode.OK });
            }
            catch(Exception exc)
            {
                return Json(new { status = HttpStatusCode.InternalServerError, message = exc.Message });
            }
        }
    }
}