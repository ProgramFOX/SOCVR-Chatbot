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
    
    public partial class RegisteredUser
    {
        public RegisteredUser()
        {
            this.CompletedAuditEntries = new HashSet<CompletedAuditEntry>();
            this.NoItemsInFilterEntries = new HashSet<NoItemsInFilterEntry>();
            this.ReviewSessions = new HashSet<ReviewSession>();
            this.ReviewItems = new HashSet<ReviewItem>();
            this.ReviewRefreshRequests = new HashSet<ReviewRefreshRequest>();
        }
    
        public int Id { get; set; }
        public int ChatProfileId { get; set; }
        public bool IsOwner { get; set; }
    
        public virtual ICollection<CompletedAuditEntry> CompletedAuditEntries { get; set; }
        public virtual ICollection<NoItemsInFilterEntry> NoItemsInFilterEntries { get; set; }
        public virtual ICollection<ReviewSession> ReviewSessions { get; set; }
        public virtual ICollection<ReviewItem> ReviewItems { get; set; }
        public virtual ICollection<ReviewRefreshRequest> ReviewRefreshRequests { get; set; }
    }
}
