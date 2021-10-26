using IdentityWallet.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWallet.SDK.Example.Models
{
    public class AddAssertionMethodRequest
    {
        public SDKOperationInstance State { get; set; }
        public string Did { get; set; }
    }
}
