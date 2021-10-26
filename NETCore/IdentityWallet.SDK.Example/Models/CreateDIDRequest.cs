using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWallet.SDK.Example.Models
{
    public class CreateDIDRequest
    {
        public string OwnerDid { get; set; }
    }

    public class CreateDIDResponse
    {
        public string NewDid { get; set; }
    }
}
