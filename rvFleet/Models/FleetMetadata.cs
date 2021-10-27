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
        [Display(Name = "Fecha de facturación")]
        public string FacFechaFactura;
        [Display(Name = "Número de factura")]
        public string FacNumeroFactura;
        [Display(Name = "Observaciones")]
        public string FacComentario;
        [Display(Name = "Pagado por")]
        public string FacUsuarioPago;
    }

    public class vehiclesMetadata
    {
        [Required]
        [Display(Name = "Número de placa")]
        [StringLength(50)]
        public string VehPlaca;
        [Required]
        [Display(Name = "Vin")]
        [StringLength(30)]
        public string VehVIN;
        [Required]
        [Display(Name = "Motor")]
        [StringLength(30)]
        public string VehMotor;
        [Required]
        [Display(Name = "Cilindraje")]
        [StringLength(15)]
        public string VehCilindraje;
        [Required]
        [Display(Name = "Año")]
        public int VehAno;
        [Required]
        [Display(Name = "Color")]
        [StringLength(50)]
        public string VehColor;
        [Required]
        [Display(Name = "Marca")]
        [StringLength(25)]
        public string VehMarca;
        [Required]
        [Display(Name = "Modelo")]
        [StringLength(25)]
        public string VehModelo;
        [Required]
        [Display(Name = "Código alterno")]
        [StringLength(50)]
        public string VehCodigoAresSun;
        [Display(Name = "Taller")]
        public int VehCodigoProveedor;
        [Display(Name = "Empresa")]
        [StringLength(250)]
        public string VehCodigoEmpresa;
        [Required]
        [Display(Name = "Fecha de matrícula")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string VehFechaMatricula;
        [Display(Name = "Número de poliza")]
        [StringLength(50)]
        public string VehNumeroPoliza;
        [Display(Name = "Foto de revisión")]
        [StringLength(500)]
        public string VehUrlFotoRevision;
        [Display(Name = "Encargado")]
        [StringLength(50)]
        public string VehCodigoUsuario;
        [Required]
        [Display(Name = "Kilometraje actual")]
        public int? VehKilometraje;
        [Display(Name = "Fecha de act. KMs")]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? VehKilometrajeActualizado;
    }
}