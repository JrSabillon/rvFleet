using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.POCO
{
    public class FacturaHistorico
    {
        public string FechaPago { get; set; }
        public string FechaFacturo { get; set; }
        public string UsuarioPago { get; set; }
        public string Empresa { get; set; }
        public string NumeroFactura { get; set; }
        public string Detalle { get; set; }
        public string Valor { get; set; }
        public string Vehiculo { get; set; }
        public string Kilometraje { get; set; }
        public string Rubro { get; set; }
        public string FechaEntregaAdmin { get; set; }
        public string Observaciones { get; set; }
    }
}