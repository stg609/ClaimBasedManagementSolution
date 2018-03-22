using System;
using Domain;

namespace IdServer.Domain
{
    public class ClaimDTO : BaseEntity<int>
    {
        public string OwnerIdentity { get; set; }
        public string OwnerIp { get; set; }

        public string Type { get; set; }
        public string Value { get; set; }

    }
}
