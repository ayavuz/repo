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
    
    public partial class Artikel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Artikel()
        {
            this.ArtikelBons = new HashSet<ArtikelBon>();
        }
    
        public long ID { get; set; }
        public string Omschrijving { get; set; }
        public string Afbeelding { get; set; }
        public long Categorie { get; set; }
        public long PrijsEuro { get; set; }
        public long PrijsCent { get; set; }
        public long Actief { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArtikelBon> ArtikelBons { get; set; }
        public virtual Categorie Categorie1 { get; set; }
    }
}
