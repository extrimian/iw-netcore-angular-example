using IdentityWallet.SDK.Models;

namespace IdentityWallet.SDK.Example.Models
{
    public class TokenRequest
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public long TokenId { get; set; }
        public SDKOperationInstance State { get; set; }
    }
}
