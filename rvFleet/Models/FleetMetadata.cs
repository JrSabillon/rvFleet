using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace rvFleet.Models
{
    public class facturasMetadata
    {
        [Display(Name = "Fecha de orden")]
        [DataType(DataType.DateTime)]
        public DateTime FacFechaOrden;
        [Display(Name = "Proveedor")]
        public int FacCodigoProveedor;
        [Display(Name = "Kilometraje")]
        public string FacKilometraje;
        [Display(Name = "Fecha de facturación")]
        public string FacFechaFactura;
        [Display(Name = "Numero de factura")]
        public string FacNumeroFactura;
    }
}