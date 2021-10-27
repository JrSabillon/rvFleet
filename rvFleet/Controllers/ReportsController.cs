using Newtonsoft.Json;
using OfficeOpenXml;
using rvFleet.Models;
using rvFleet.POCO.Reports;
using rvFleet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace rvFleet.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        ReportsViewModel ReportsViewModel;
        public ReportsController()
        {
            ReportsViewModel = new ReportsViewModel();
        }

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Vehicles()
        {
            try
            {
                var model = new ReportsModel();
                int CarsAssigned, CarsUnassigned, TotalCars;
                ReportsViewModel.CarInventory(out CarsAssigned, out CarsUnassigned, out TotalCars);

                ViewBag.CarsAssigned = CarsAssigned;
                ViewBag.CarsUnassigned = CarsUnassigned;
                ViewBag.TotalCars = TotalCars;
                model.VehicleCosts = ReportsViewModel.GetVehicleCosts();
                model.RecommendedMaintenances = ReportsViewModel.GetRecommendedMaintenance()
                    .Where(x => x.DistanciaRecorrida > x.DistanciaCambio && 100 - (x.DistanciaRecorrida / x.DistanciaCambio * 100) < 30).ToList();
                model.VehicleAnualCostGraphData = ReportsViewModel.VehicleAnualCosts(DateTime.Now.Year);
                model.KilometrajesPorVehiculoAnoActual = ReportsViewModel.GetKilometrajePorVehiculoAnoActual();
                //model.VidaUtilData = this.VidaUtilData(null);

                return View(model);
            }
            catch(Exception exc)
            {
                throw exc;
                //ViewBag.Message = exc.Message;
                //return View("Error");
            }
        }

        public ActionResult PartsCost(string startDate, string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.ToString("yyyy-MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetPartsCost_Filtered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult OrdersDetails(string VehPlaca, string startDate, string endDate)
        {
            ViewBag.VehPlaca = VehPlaca;
            ViewBag.startDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd");
            ViewBag.endDate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd");
            var model = ReportsViewModel.GetFacturasByVehicleAndDate(VehPlaca, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            
            foreach (var item in model)
            {
                //List<detallefactura> detallefacturas = viewModel.GetDetallefacturas(model[i].FacCodigoOrden);
                List<detallefactura> detallefacturas = item.detallefactura.ToList();
                List<string> OrderElements = new List<string>();


                for (int j = 0; j < detallefacturas.Count; j++)
                {
                    OrderElements.Add($"{detallefacturas[j].DetCantidad} {detallefacturas[j].DetDescripcion}");
                }

                item.Detalles = string.Join(", ", OrderElements);
                item.NombreProveedor = item.proveedor.ProNombre;
                item.proveedor.facturas = null;
                item.proveedor.vehiculos = null;
            }

            ViewBag.Total = model.Sum(x => x.FacValorFactura);

            return View(model.ToList());
        }

        public ActionResult NextMaintenance(string VehPlaca)
        {
            try
            {
                VehiclesViewModel VehiclesViewModel = new VehiclesViewModel();

                var vehiculos = VehiclesViewModel.GetVehiculos();
                ViewBag.VehPlaca = new SelectList(vehiculos, "VehPlaca", "DisplayText", VehPlaca ?? string.Empty);

                List<GetVehicleGraphData_Result> data = VehiclesViewModel.GetGraphData(VehPlaca ?? vehiculos.FirstOrDefault().VehPlaca);
                ViewBag.RecommendedMaintenances = ReportsViewModel.GetRecommendedMaintenance().OrderBy(x => x.Prioridad).ToList();
                //ViewBag.JsonData = JsonConvert.SerializeObject(data);

                return View(data);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public ActionResult VehiclesCosts(string startDate, string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetVehicleCostsFiltered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return View(model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return View("Error");
            }
        }

        public ActionResult _VehiclesTableCost(string startDate, string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetVehicleCostsFiltered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return PartialView("_VehiclesTableCost", model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return PartialView("Error");
            }
        }

        public ActionResult _RubrosTableCost(string startDate, string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetRubrosTableCost(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return PartialView("_RubrosTableCost", model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return PartialView("Error");
            }
        }

        public ActionResult _ProveedoresTableCost(string startDate, string endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
                    endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
                }

                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                var model = ReportsViewModel.GetProveedoresTableCost(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

                return PartialView("_ProveedoresTableCost", model);
            }
            catch (Exception exc)
            {
                ViewBag.Message = exc.Message;
                return PartialView("Error");
            }
        }

        public FileResult ExportVehiclesCostResume(string data)
        {
            var model = JsonConvert.DeserializeObject<List<vehiclecosts>>(data);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();

            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Costos resumidos");
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1"].Value = "Placa";
            Sheet.Cells["B1"].Value = "Promedio mensual";
            Sheet.Cells["C1"].Value = "Total";
            
            int row = 2;
            foreach (var item in model)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.DetPlacaVehiculo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.PromedioMensual;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "L#,##0.00";
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Total;
                
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();

            return File(Ep.GetAsByteArray(), "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ResumenCostosVehiculos_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx");
        }

        public FileResult ExportVehicleCostsFiltered(string data)
        {
            var model = JsonConvert.DeserializeObject<List<spGetVehicleCostsFiltered_Result>>(data);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();

            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Costos vehiculares");
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1"].Value = "Nombre";
            Sheet.Cells["B1"].Value = "Fecha";
            Sheet.Cells["C1"].Value = "Total";

            int row = 2;
            foreach (var item in model)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Nombre;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.FacFechaOrden.ToString("yyyy-MM");
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "L#,##0.00";
                Sheet.Cells[string.Format("C{0}", row)].Value = item.CostoTotal;

                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();

            return File(Ep.GetAsByteArray(), "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"CostosVehiculos_{DateTime.Now:yyyyMMddHHmmssffff}.xlsx");
        }

        public FileResult ExportOrderDetails(string data, string VehPlaca)
        {
            var model = JsonConvert.DeserializeObject<List<facturas>>(data);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();

            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Facturas");
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1"].Value = "Factura";
            Sheet.Cells["B1"].Value = "Fecha";
            Sheet.Cells["C1"].Value = "Proveedor";
            Sheet.Cells["D1"].Value = "Detalle";
            Sheet.Cells["E1"].Value = "Vehiculos";
            Sheet.Cells["F1"].Value = "Total";

            int row = 2;
            foreach (var item in model)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.FacNumeroFactura;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.FacFechaOrden.Value.ToString("yyyy-MM");
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NombreProveedor;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Detalles;
                Sheet.Cells[string.Format("E{0}", row)].Value = VehPlaca;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "L#,##0.00";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.FacValorFactura;
                
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();

            return File(Ep.GetAsByteArray(), "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"FacturasVehiculo_{VehPlaca}_{DateTime.Now:yyyyMMddHHmmssffff}.xlsx");
        }

        public FileResult ExportNextMaintenance(string data)
        {
            var model = JsonConvert.DeserializeObject<List<recommendedmaintenance>>(data);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();

            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Mantenimientos");
            Sheet.Cells["A1:I1"].Style.Font.Bold = true;
            Sheet.Cells["A1"].Value = "Vehiculo";
            Sheet.Cells["B1"].Value = "Rubro";
            Sheet.Cells["C1"].Value = "Fecha de orden";
            Sheet.Cells["D1"].Value = "Vida util";
            Sheet.Cells["E1"].Value = "Kilometraje facturado";
            Sheet.Cells["F1"].Value = "Kilometraje actual";
            Sheet.Cells["G1"].Value = "Distancia recorrida";
            Sheet.Cells["H1"].Value = "Kilometros restantes";
            Sheet.Cells["I1"].Value = "Porcentaje";

            int row = 2;
            foreach (var item in model)
            {
                
                var color = System.Drawing.ColorTranslator.FromHtml(item.HexBackgroundColor);

                Sheet.Cells[string.Format("A{0}", row)].Value = item.DetPlacaVehiculo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.NombreRubro;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd";
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FacFechaOrden;
                Sheet.Cells[string.Format("D{0}:H{0}", row)].Style.Numberformat.Format = "#,##0";
                Sheet.Cells[string.Format("D{0}", row)].Value = item.DistanciaCambio;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.KilometrajeFacturado;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.KilometrajeActual;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.DistanciaRecorrida;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.VidaRestante;
                Sheet.Cells[string.Format("H{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells[string.Format("H{0}", row)].Style.Fill.BackgroundColor.SetColor(color);
                Sheet.Cells[string.Format("I{0}", row)].Value = item.porcentaje + "%";
                Sheet.Cells[string.Format("I{0}", row)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells[string.Format("I{0}", row)].Style.Fill.BackgroundColor.SetColor(color);

                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();

            return File(Ep.GetAsByteArray(), "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Mantenimientos_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx");
        }

        //public ActionResult VehiclesCosts(string startDate, string endDate)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        //        {
        //            startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
        //            endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
        //        }

        //        ViewBag.startDate = startDate;
        //        ViewBag.endDate = endDate;
        //        var model = ReportsViewModel.GetVehicleCostsFiltered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

        //        return View(model);
        //    }
        //    catch (Exception exc)
        //    {
        //        ViewBag.Message = exc.Message;
        //        return View("Error");
        //    }
        //}

        //public ActionResult _VehiclesTableCost(string startDate, string endDate)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        //        {
        //            startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
        //            endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
        //        }

        //        ViewBag.startDate = startDate;
        //        ViewBag.endDate = endDate;
        //        var model = ReportsViewModel.GetVehicleCostsFiltered(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

        //        return PartialView("_VehiclesTableCost", model);
        //    }
        //    catch (Exception exc)
        //    {
        //        ViewBag.Message = exc.Message;
        //        return PartialView("Error");
        //    }
        //}

        //public ActionResult _RubrosTableCost(string startDate, string endDate)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        //        {
        //            startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
        //            endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
        //        }

        //        ViewBag.startDate = startDate;
        //        ViewBag.endDate = endDate;
        //        var model = ReportsViewModel.GetRubrosTableCost(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

        //        return PartialView("_RubrosTableCost", model);
        //    }
        //    catch (Exception exc)
        //    {
        //        ViewBag.Message = exc.Message;
        //        return PartialView("Error");
        //    }
        //}

        //public ActionResult _ProveedoresTableCost(string startDate, string endDate)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        //        {
        //            startDate = DateTime.Now.Year + "-" + DateTime.Now.Date.ToString("MM") + "-01";//La fecha inicial de este mes.
        //            endDate = DateTime.Now.Date.ToString("yyyy-MM-dd"); //La fecha de hoy
        //        }

        //        ViewBag.startDate = startDate;
        //        ViewBag.endDate = endDate;
        //        var model = ReportsViewModel.GetProveedoresTableCost(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

        //        return PartialView("_ProveedoresTableCost", model);
        //    }
        //    catch (Exception exc)
        //    {
        //        ViewBag.Message = exc.Message;
        //        return PartialView("Error");
        //    }
        //}
    }
}