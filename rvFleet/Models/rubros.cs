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
    
    public partial class rubros
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rubros()
        {
            this.rubrodetalle = new HashSet<rubrodetalle>();
        }
    
        public int CodigoRubro { get; set; }
        public string NombreRubro { get; set; }
        public Nullable<int> DistanciaCambio { get; set; }
        public string ColorIdentificador { get; set; }
        public Nullable<int> Prioridad { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rubrodetalle> rubrodetalle { get; set; }
    }
}
