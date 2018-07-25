﻿using System;
using System.Collections.Generic;

namespace Toucan.Data.Model
{
    public partial class Role : IAuditable
    {
        public Role()
        {
            this.Users = new HashSet<UserRole>();
            this.SecurityClaims= new HashSet<RoleSecurityClaim>();
        }

        public string RoleId { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public virtual ICollection<RoleSecurityClaim> SecurityClaims { get; set; }
        public virtual ICollection<UserRole> Users { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual User LastUpdatedByUser { get; set; }
    }
}
