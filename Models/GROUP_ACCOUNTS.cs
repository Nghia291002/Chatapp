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
    
    public partial class GROUP_ACCOUNTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GROUP_ACCOUNTS()
        {
            this.MESSAGEs = new HashSet<MESSAGE>();
        }
    
        public decimal GROUP_ACCOUNT_ID { get; set; }
        public Nullable<decimal> USER_ID { get; set; }
        public Nullable<decimal> GROUP_ID { get; set; }
    
        public virtual GROUP GROUP { get; set; }
        public virtual USER USER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MESSAGE> MESSAGEs { get; set; }
    }
}
