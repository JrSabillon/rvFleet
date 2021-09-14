using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using rvFleet.App_Code;

namespace rvFleet.Models
{
    [MetadataType(typeof(facturasMetadata))]
    public partial class facturas
    {
        public string Detalles { get; set; }
        public string NombreProveedor { get; set; }
        //public string facNumeroFactura 
        //{
        //    get => FacNumeroFactura != null ? FacNumeroFactura : string.Empty;
        //}
    }

    public partial class detallefactura
    {
        public detallefactura()
        {
            //facturas = new facturas();
        }

        public string NombreRubro { get; set; }
    }

    [MetadataType(typeof(vehiclesMetadata))]
    public partial class vehiculos
    {
        public string NombreEmpresa { get; set; }
        public string NombreEncargado { get; set; }
        public HttpPostedFileBase ImgRevision { get; set; }
        public HttpPostedFileBase ImgDelantera { get; set; }
        public HttpPostedFileBase ImgIzquierda { get; set; }
        public HttpPostedFileBase ImgDerecha { get; set; }
        public HttpPostedFileBase ImgTrasera { get; set; }
        public HttpPostedFileBase ImgMotor { get; set; }
        public HttpPostedFileBase ImgInterior { get; set; }
        //public List<spGetVehiclesCostTableValues_Result> Costs { get; set; } = new List<spGetVehiclesCostTableValues_Result>();
    }

    [MetadataType(typeof(vehiclesMetadata))]
    public partial class vehiclefulldata { }

    public partial class GetVehicleGraphData_Result
    {
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string backgroundColor
        {
            get
            {
                if (Porcentaje <= 1)
                    return "rgba(192, 0, 0, 0.7)";
                else if (Porcentaje > 0 && Porcentaje <= 20)
                    return "rgba(255, 0, 0, 0.7)";
                else if (Porcentaje > 20 && Porcentaje <= 40)
                    return "rgba(255, 192, 0, 0.7)";
                else if (Porcentaje > 40 && Porcentaje <= 60)
                    return "rgba(169, 208, 142, 0.7)";
                else if (Porcentaje > 60 && Porcentaje <= 80)
                    return "rgba(146, 208, 80, 0.7)";
                else
                    return "rgba(0, 176, 80, 0.7)";
            }
        }
    }

    public partial class GetVehicleAnualCostsGraphData_Result
    {
        public string FormatedNumber => GlobalFunctions.FormatThousandsAndMillions(FacValorFactura);
    }

    public partial class GetVehicleCosts_Result
    {
        public string FormatedNumber => GlobalFunctions.FormatThousandsAndMillions(VALOR);
    }

    public partial class GetPartsCost_Result
    {
        public string FormatedNumber => GlobalFunctions.FormatThousandsAndMillions(Total);
    }

    public partial class recommendedmaintenance
    {
        public string backgroundColor
        {
            get
            {
                if(porcentaje <= 1) 
                    return "rgba(192, 0, 0, 0.7)";
                else if(porcentaje > 0 && porcentaje <= 20)
                    return "rgba(255, 0, 0, 0.7)";
                else if(porcentaje > 20 && porcentaje <= 40)
                    return "rgba(255, 192, 0, 0.7)";
                else if(porcentaje > 40 && porcentaje <= 60)
                    return "rgba(169, 208, 142, 0.7)";
                else if(porcentaje > 60 && porcentaje <= 80)
                    return "rgba(146, 208, 80, 0.7)";
                else
                    return "rgba(0, 176, 80, 0.7)";
            }
        }

        public double porcentaje
        {
            get
            {
                if(DistanciaRecorrida > DistanciaCambio)
                {
                    return 0;
                }

                return Math.Round((float)(100 - (((float)DistanciaRecorrida / (float)DistanciaCambio) * 100)), 2);
            }
        }
    }

    public partial class bitacoravehiculo
    {
        public string bitacoraTipoDescripcion { get; set; }
    }
}