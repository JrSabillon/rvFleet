using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.POCO
{
    public class InspectionsTable
    {
        public string VehPlaca { get; set; }
        public DateTime Fecha { get; set; }
        public int VehCodigoVehiculo { get; set; }
        public string CodigoUsuario { get; set; }
        public int Kilometraje { get; set; }
        public string Observacion { get; set; }
    }
}