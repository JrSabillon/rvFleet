using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.POCO
{
    public class VehicleModel
    {
        public int VehCodigoVehiculo { get; set; }
        public string VehPlaca { get; set; }
        public string VehVIN { get; set; }
        public string VehMotor { get; set; }
        public string VehCilindraje { get; set; }
        public int VehAno { get; set; }
        public string VehColor { get; set; }
        public string VehMarca { get; set; }
        public string VehModelo { get; set; }
        public string VehCodigoAresSun { get; set; }
        public Nullable<int> VehCodigoProveedor { get; set; }
        public string VehCodigoEmpresa { get; set; }
        public System.DateTime VehFechaMatricula { get; set; }
        public string VehNumeroPoliza { get; set; }
        public string VehUrlFotoRevision { get; set; }
        public string VehCodigoUsuario { get; set; }
        ///
        public string NombreEmpresa { get; set; }
        public string NombreEncargado { get; set; }
    }
}