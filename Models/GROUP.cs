//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chatapp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GROUP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GROUP()
        {
            this.GROUP_ACCOUNTS = new HashSet<GROUP_ACCOUNTS>();
        }
    
        public decimal GROUP_ID { get; set; }
        public string USERNAME { get; set; }
        public string GROUP_NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUP_ACCOUNTS> GROUP_ACCOUNTS { get; set; }
    }
}
