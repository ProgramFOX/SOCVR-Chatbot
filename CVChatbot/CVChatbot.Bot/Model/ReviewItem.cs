//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CVChatbot.Bot.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReviewItem
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public Nullable<bool> AuditPassed { get; set; }
        public string PostTag1 { get; set; }
        public string PostTag2 { get; set; }
        public string PostTag3 { get; set; }
        public string PostTag4 { get; set; }
        public string PostTag5 { get; set; }
    
        public virtual RegisteredUser RegisteredUser { get; set; }
    }
}
