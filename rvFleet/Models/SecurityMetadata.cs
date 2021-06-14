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
        [StringLength(15)]
        public string Contrasena;
    }
}