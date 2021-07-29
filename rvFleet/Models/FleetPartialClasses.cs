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
    }

    [MetadataType(typeof(vehiclesMetadata))]
    public partial class vehiclefulldata { }

    public partial class GetVehicleGraphData_Result
    {
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
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
}