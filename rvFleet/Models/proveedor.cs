//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace rvFleet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class proveedor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public proveedor()
        {
            this.facturas = new HashSet<facturas>();
            this.vehiculos = new HashSet<vehiculos>();
        }
    
        public int ProCodigoProveedor { get; set; }
        public string ProNombre { get; set; }
        public string ProNumeroContacto { get; set; }
        public string ProNombreContacto { get; set; }
        public string ProRTN { get; set; }
        public string ProUbicacion { get; set; }
        public string ProCuentaBancaria { get; set; }
        public string ProNombreBanco { get; set; }
        public string ProTipo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<facturas> facturas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<vehiculos> vehiculos { get; set; }
    }
}
