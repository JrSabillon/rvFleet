using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rvFleet.Models
{
    [MetadataType(typeof(facturasMetadata))]
    public partial class facturas
    {
        public string Detalles { get; set; }
        public string NombreProveedor { get; set; }
    }
}