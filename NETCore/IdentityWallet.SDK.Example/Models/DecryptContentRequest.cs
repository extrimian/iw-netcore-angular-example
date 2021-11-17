using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWallet.SDK.Example.Models
{
    public class DecryptContentRequest
    {
        public string Content { get; set; }
        public string Did { get; set; }
    }
}