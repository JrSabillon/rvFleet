using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rvFleet.Models
{
    public class usuarioMetadata
    {
        [Required]
        [Display(Name = "Usuario")]
        [DataType(DataType.Text)]
        public string IdUsuario;
        [Required]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contrasena;
        [Display(Name = "Empresa")]
        public string IdEmpresa;
        [Display(Name = "Código de empleado")]
        public string CodigoRRHH;
        [Required]
        [Display(Name = "Nombre completo")]
        public string NombreUsuario;
        [Required]
        [Display(Name = "# identidad")]
        public string IdentidadUsuario;
        [Required]
        [Display(Name = "Correo")]
        [DataType(DataType.EmailAddress)]
        public string Correo;
    }
}