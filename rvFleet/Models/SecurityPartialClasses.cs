using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rvFleet.Models
{
    [MetadataType(typeof(usuarioMetadata))]
    public partial class usuario {
        public bool MultipleVehicles { get; set; }
    }
}