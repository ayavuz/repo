//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfBonApp.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bon()
        {
            this.ArtikelBons = new HashSet<ArtikelBon>();
        }
    
        public long ID { get; set; }
        public string BonDT { get; set; }
        public string OphalenDT { get; set; }
        public string KlantNaam { get; set; }
        public string KlantAdres { get; set; }
        public string KlantNummer { get; set; }
        public string BetaaldDT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArtikelBon> ArtikelBons { get; set; }
    }
}