//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LookupAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContactDetail
    {
        public System.Guid ContactDetailId { get; set; }
        public System.Guid ContactId { get; set; }
        public System.Guid ContactTypeId { get; set; }
        public string ContactDetails { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<bool> IsTollFree { get; set; }
        public Nullable<bool> IsPerson { get; set; }
        public string Conditions { get; set; }
        public string Icon { get; set; }
    
        public virtual Contact Contact { get; set; }
        public virtual ContactType ContactType { get; set; }
    }
}