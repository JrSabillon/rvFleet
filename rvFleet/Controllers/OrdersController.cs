﻿using rvFleet.Models;
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
using System.Data;
using ClosedXML.Excel;
using OfficeOpenXml;
using System.Configuration;
using iTextSharp.text.pdf;
using System.Drawing;
using iTextSharp.text;
using System.Text;
using Image = iTextSharp.text.Image;

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

        public void DownloadExcel_Facturas(string searchString, string VehPlaca, string type = "1")
        {
            try
            {
                List<facturas> model = new List<facturas>();
            
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
                    model = model.Where(x => x.proveedor.ProNombre.ToLower().Contains(searchString.ToLower()) || x.detallefactura.Any(y => y.DetDescripcion.ToLower().Contains(searchString.ToLower())) /*|| x.facNumeroFactura.Contains(searchString)*/).ToList();

                if (!string.IsNullOrEmpty(VehPlaca))
                    model = model.Where(x => x.detallefactura.Any(y => y.DetPlacaVehiculo.Equals(VehPlaca))).ToList();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage Ep = new ExcelPackage();

                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Facturas");
                Sheet.Cells["A1:I1"].Style.Font.Bold = true;
                Sheet.Cells["A1"].Value = "# Orden";
                Sheet.Cells["B1"].Value = "Factura";
                Sheet.Cells["C1"].Value = "Fecha Orden";
                Sheet.Cells["D1"].Value = "Fecha Factura";
                Sheet.Cells["E1"].Value = "Proveedor";
                Sheet.Cells["F1"].Value = "Detalle";
                Sheet.Cells["G1"].Value = "Vehículos";
                Sheet.Cells["H1"].Value = "Total";
                Sheet.Cells["I1"].Value = "Facturado";

                int row = 2;
                foreach (var item in model)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.FacCodigoOrden;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.FacNumeroFactura;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FacFechaOrden;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd";
                    if (item.FacFechaFactura.HasValue)
                    {
                        Sheet.Cells[string.Format("D{0}", row)].Value = item.FacFechaFactura;
                        Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd";
                    }
                    else
                        Sheet.Cells[string.Format("D{0}", row)].Value = "SIN FECHA DE FACTURA";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.proveedor.ProNombre;
                    Sheet.Cells[string.Format("F{0}", row)].Value = string.Join(", ", item.detallefactura.Select(x => x.DetDescripcion).ToList());
                    Sheet.Cells[string.Format("G{0}", row)].Value = string.Join(", ", item.detallefactura.Select(x => x.DetPlacaVehiculo).Distinct().ToList());
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.FacValorFactura;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "L#,##0.00";
                    if(string.IsNullOrEmpty(item.FacNumeroFactura))
                        Sheet.Cells[string.Format("I{0}", row)].Value = "NO";
                    else
                        Sheet.Cells[string.Format("I{0}", row)].Value = "SI";

                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + $"Facturas_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
            }
            catch { }
        }

        // GET: Orders
        public ActionResult Index(string searchString, int? page, string VehPlaca, int? pageSize, string sortOrder = "orden_desc", string type = "1")
        {
            try
            {
                pageSize = pageSize.HasValue ? pageSize : Convert.ToInt32(ConfigurationManager.AppSettings["PaginationSize"].ToString());
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
                    case "4":
                        model = viewModel.GetFacturasNoEntregadas();
                        pageSize = model.Count;
                        break;
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    ViewBag.searchString = searchString;
                    model = model.Where(x => x.proveedor.ProNombre.ToLower().Contains(searchString.ToLower()) || x.detallefactura.Any(y => y.DetDescripcion.ToLower().Contains(searchString.ToLower())) /*|| x.facNumeroFactura.Contains(searchString)*/).ToList();
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
                    case "orden":
                        model = model.OrderBy(x => x.FacCodigoOrden).ToList();
                        break;
                    case "orden_desc":
                        model = model.OrderByDescending(x => x.FacCodigoOrden).ToList();
                        break;
                    default:
                        break;
                }

                ViewBag.SortOrder = sortOrder; //Para saber que icono mostrar
                ViewBag.SortFacCodigoOrden = sortOrder == "orden" ? "orden_desc" : "orden";
                ViewBag.SortDate = sortOrder == "fecha" ? "fecha_desc" : "fecha";
                ViewBag.SortFacNumeroFactura = sortOrder == "factura" ? "factura_desc" : "factura";
                ViewBag.SortProveedor = sortOrder == "proveedor" ? "proveedor_desc" : "proveedor";
                ViewBag.SortTotal = sortOrder == "total" ? "total_desc" : "total";

                ViewBag.Type = new SelectList(GetTypes(), "Value", "Text", type);
                ViewBag.PageSize = pageSize;
                return View(model.ToPagedList(pageNumber, pageSize.Value));
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
                new ItemModel { Text = "Pendiente entrega", Value = "4" }
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
                                DetCodigoRubro = Convert.ToInt32(row.Cell(13).Value),
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

        public FileResult GenerateOrdersPDF(List<facturas> facturas, string Source)
        {
            var data = facturas.Where(x => x.IsChecked).ToList();
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;

            string FileName = string.Format("ReporteFacturas_{0}.pdf", dTime.ToString("yyyyMMddHHmmss"));
            Document doc = new Document(PageSize.A4.Rotate());
            doc.SetMargins(25f, 75f, 25f, 0f);
            string LogoPath = Server.MapPath("~");
            Image logo = Image.GetInstance(LogoPath + "/Content/images/logos/rv_mini.jpg");
            Image logoDMS = Image.GetInstance(LogoPath + "/Content/images/logos/dms.jpg");
            logo.Alignment = Element.ALIGN_LEFT;
            //logo.Width = 100f;
            PdfPTable tableLayout = new PdfPTable(7);

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            PdfPTable table = AddTableContent(data, tableLayout);
            Paragraph logos = AddLogos(new Image[] { logo, logoDMS });
            doc.Add(logos);
            doc.Add(table);
            Paragraph numFacturas = new Paragraph($"Número de facturas: {data.Count}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f));
            numFacturas.Alignment = Element.ALIGN_RIGHT;
            Paragraph total = new Paragraph($"Gastos totales: {data.Sum(x => x.FacValorFactura):N2}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f));
            total.Alignment = Element.ALIGN_RIGHT;
            doc.Add(numFacturas);
            doc.Add(total);
            
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            if (Source.ToLower().Equals("admon"))
            {
                //ACTUALIZAR FECHA DE ENTREGA A ADMINISTRACION.
                foreach (var factura in data)
                {
                    viewModel.updateFechaAdmon(factura);
                }
            }

            return File(workStream, "application/pdf", FileName);
        }

        protected Paragraph AddLogos(Image[] images)
        {
            Paragraph p = new Paragraph();
            
            p.Add(new Chunk(images[0], 0, 0, true));
            p.Add(new Chunk(images[1], 600, 0, true));
            
            return p;
        }

        protected PdfPTable AddTableContent(List<facturas> facturas, PdfPTable tableLayout)
        {
            float[] headers = { 13, 40, 25, 40, 100, 50, 25 };
            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;

            tableLayout.AddCell(new PdfPCell(new Phrase("Reporte de facturas"))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5, 
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "#");
            AddCellToHeader(tableLayout, "Factura");
            AddCellToHeader(tableLayout, "Fecha");
            AddCellToHeader(tableLayout, "Proveedor");
            AddCellToHeader(tableLayout, "Detalle");
            AddCellToHeader(tableLayout, "Vehículos");
            AddCellToHeader(tableLayout, "Total");

            foreach (var factura in facturas)
            {
                double subtotal = Array.ConvertAll(factura.ValorDetalle.Split('|'), double.Parse).ToList().Sum();
                double total = factura.FacAplicaImpuesto.Value ? subtotal * 0.15 + subtotal : subtotal;
                AddCellToBody(tableLayout, factura.FacCodigoOrden.ToString());
                AddCellToBody(tableLayout, factura.FacNumeroFactura);
                AddCellToBody(tableLayout, factura.FacFechaOrden.Value.ToString("yyyy-MM-dd"));
                AddCellToBody(tableLayout, factura.proveedor.ProNombre);
                AddCellToBody(tableLayout, factura.Detalles);
                AddCellToBody(tableLayout, factura.Placas);
                AddCellToBody(tableLayout, total.ToString("N2"));
            }

            return tableLayout;
        }

        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText)){
                HorizontalAlignment = Element.ALIGN_LEFT, 
                Padding = 5,
                BackgroundColor = new BaseColor(171, 204, 217)
            });
        }

        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT, 
                Padding = 5, 
                BackgroundColor = new BaseColor(255, 255, 255)
            });
        }
    }
}