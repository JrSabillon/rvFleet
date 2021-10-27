using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rvFleet.POCO
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Usuario")]
        [DataType(DataType.Text)]
        public string IdUsuario { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
        public string IdEmpresa { get; set; }
        public string NombreUsuario { get; set; }
        public bool MultipleVehicles { get; set; } = false;
    }
}