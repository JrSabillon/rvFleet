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
    
    public partial class privilegio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public privilegio()
        {
            this.rol = new HashSet<rol>();
        }
    
        public string IdPrivilegio { get; set; }
        public string NombrePrivilegio { get; set; }
        public string DescripcionPrivilegio { get; set; }
        public Nullable<bool> EstadoPrivilegio { get; set; }
        public Nullable<int> NivelPrivilegio { get; set; }
        public string PadrePrivilegio { get; set; }
        public string URL { get; set; }
        public string Parametros { get; set; }
        public string Icono { get; set; }
    
        public virtual privilegiopermiso privilegiopermiso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rol> rol { get; set; }
    }
}
