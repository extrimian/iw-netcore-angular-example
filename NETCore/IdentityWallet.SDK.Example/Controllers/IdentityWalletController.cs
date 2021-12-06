using Extrimian.DecentralizedIdentity;
using Extrimian.DecentralizedIdentity.Models;
using Extrimian.DecentralizedIdentity.Models.Registry.Owners;
using Extrimian.DecentralizedIdentity.Models.Relationship;
using IdentityWallet.SDK.Example.Models;
using IdentityWallet.SDK.Models;
using IdentityWallet.SDK.Models.DIDCommMessages;
using IdentityWallet.SDK.Models.Requests;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityWallet.SDK.Example.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityWalletController : ControllerBase
    {
        private IdentityWalletSDK IdentityWalletSDK;

        private string DAPP_DID = "did:ethr:rsk:testnet:0x8Bfea8277FD3acB2754cFaAc4c30c7010dA225FE"; // = "did:ethr:rsk:0x1AC246974C1751a7FCA08ceAFf04Af0007f3bf8E";
        private string IW_DID = "did:ethr:rsk:testnet:0xdb2F915aac01168680d72518F210D91cbE833882"; // = "did:ethr:rsk:0xF3Fb96359A2586FD308aB1fe1B86BE3EA17b5F57";
        private string IW_VM = "did:ethr:rsk:testnet:0xdb2F915aac01168680d72518F210D91cbE833882#delegate-1"; // = "did:ethr:rsk:0xF3Fb96359A2586FD308aB1fe1B86BE3EA17b5F57#delegate-1";

        private string API_WALLET_USERNAME = "dappdemo@extrimian.com"; // = "anhmain@extrimian.com";
        private string API_WALLET_PWD = "Pa$$word1"; // = "VMDwyAVdnh5N8b!b4MXQy-XHE$NSKLwp";

        private string API_URL = "http://localhost:3005";
        private string DID_API_URL = "http://localhost:3015";
        private string SDK_API_URL = "http://localhost:3018";
        private string DID_API_KEY = "";
        private string SDK_API_KEY = "";
        private string API_KEY = "";

        public APIWallet APIWallet { get; set; }

        public IdentityWalletController()
        {
            DAPP_DID = Environment.GetEnvironmentVariable("DAPP_DID");
            IW_DID = Environment.GetEnvironmentVariable("IW_DID");
            IW_VM = Environment.GetEnvironmentVariable("IW_VM");

            API_WALLET_USERNAME = Environment.GetEnvironmentVariable("API_WALLET_USERNAME");
            API_WALLET_PWD = Environment.GetEnvironmentVariable("API_WALLET_PWD");

            DID_API_URL = Environment.GetEnvironmentVariable("DID_API_URL");
            DID_API_KEY = Environment.GetEnvironmentVariable("DID_API_KEY");

            SDK_API_URL = Environment.GetEnvironmentVariable("SDK_API_URL");
            SDK_API_KEY = Environment.GetEnvironmentVariable("SDK_API_KEY");

            API_URL = Environment.GetEnvironmentVariable("API_URL");
            API_KEY = Environment.GetEnvironmentVariable("API_KEY");

            IdentityWalletSDK = new IdentityWalletSDK(DAPP_DID, IW_DID, IW_VM, DIDCommPack, DIDCommUnpack, LoggedIn, SDK_API_KEY, SDK_API_URL);

            APIWallet = new APIWallet(API_WALLET_USERNAME, API_WALLET_PWD, API_KEY, API_URL);
        }

        [HttpPost("create-did-change-owner")]
        public async Task<ActionResult<CreateDIDResponse>> CreateDIDChangeOwner(CreateDIDRequest request)
        {
            var registry = new ExtrimianRegistry(DIDCommPack, DIDCommUnpack, DID_API_KEY, DID_API_URL);

            var newDID = await registry.CreateDIDControlledBy(request.OwnerDid);

            return new CreateDIDResponse
            {
                NewDid = newDID
            };
        }

        [HttpPost("add-assertion-method")]
        public async Task<ActionResult<SDKCommunicationMessage>> AddAssertionMethod(AddAssertionMethodRequest request)
        {
            var registry = new ExtrimianRegistry(DIDCommPack, DIDCommUnpack, DID_API_KEY, DID_API_URL);

            var content = await registry.GetAddAssertionMethodData(request.Did, new AssertionMethodData
            {
                Algorithm = AlgorithmType.Secp256k1,
                Base = Base.Hex,
                ExpiresIn = 31556952000,
                Value = "0x5f093f1412d227bc8a34d267932b36e5eceb1edb4321d2d9964b24dd0a5b86e5"
            });
            
            return await IdentityWalletSDK.ExtrSignContent(request.State, new ExtrSignContentRequest
            {
                TemplateId = content.TemplateId,
                Content = new ExtrSignContentData
                {
                    Data = content.Data,
                },
            });
        }

        [HttpPost("process-add-assertion-method")]
        public async Task<ActionResult<SDKCommunicationMessage>> ProcessAddAssertionMethod(DecryptContentRequest request)
        {
            var content = await IdentityWalletSDK.DecryptContent(request.Content);

            var registry = new ExtrimianRegistry(DIDCommPack, DIDCommUnpack, DID_API_KEY, DID_API_URL);

            var method = await registry.AddAssertionMethod(request.Did, new AssertionMethodSignedData
            {
                Algorithm = AlgorithmType.Secp256k1,
                Base = Base.Hex,
                ExpiresIn = 31556952000,
                Value = "0x5f093f1412d227bc8a34d267932b36e5eceb1edb4321d2d9964b24dd0a5b86e5",
                SignR = content.SignedContent.R,
                SignS = content.SignedContent.S,
                SignV = int.Parse(content.SignedContent.V),
            });

            return Ok();
        }

        public async Task<string> DIDCommPack(string data)
        {
            var didComm = await APIWallet.DIDCommPack(data, true, IW_VM);
            return didComm;
        }

        public async Task<string> DIDCommUnpack(string data)
        {
            var unpacked = await APIWallet.DIDCommUnPack(data);
            return unpacked;
        }

        [HttpGet("hash-friendly-content")]
        public async Task<ActionResult<string>> HashFriendlyContent()
        {
            return await APIWallet.HashFriendlyContent("Cambiar la propiedad de la identidad 0xD9001096218e4b7c30ec1B7C85A61AfAe3e450aD al address 0x89205A3A3b2A69De6Dbf7f01ED13B2108B2c43e7");
        }

        private Task LoggedIn(LoginVC loginVC, LoginCredentialSubject subject)
        {
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", new StringValues(new[] { "X-AccessToken", "x-did" }));
            HttpContext.Response.Headers.Add("X-AccessToken", GenerateToken(subject.name, subject.did));
            HttpContext.Response.Headers.Add("x-did", subject.did);
            return Task.CompletedTask;
        }

        [HttpPost("processIWMessage")]
        public async Task<ActionResult<SDKCommunicationMessage>> ProcessMessage(SDKCommunicationMessage sdkCommunicationMessage)
        {
            var response = await IdentityWalletSDK.ProcessMessage(sdkCommunicationMessage);
            return response;
        }

        [HttpPost("handshake")]
        public async Task<ActionResult<SDKCommunicationMessage>> Handshake(SDKOperationInstance state)
        {
            return await IdentityWalletSDK.Handshake(state, IWLanguage.es_ES);
        }

        [HttpPost("vc-login")]
        public async Task<ActionResult<SDKCommunicationMessage>> VCLogin(SDKOperationInstance state)
        {
            return await IdentityWalletSDK.SignVCForLogin(state);
        }

        public string GenerateToken(string name, string did)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, did),
                    new Claim(ClaimTypes.Name, name),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("sign-content")]
        public async Task<ActionResult<SDKCommunicationMessage>> SignContent(SDKOperationInstance state)
        {
            return await IdentityWalletSDK.SignContent(state, "Este es el contenido a firmar");
        }

        [HttpPost("transfer-nft")]
        public async Task<ActionResult<SDKCommunicationMessage>> TransferNFT(TokenRequest tokenRequest)
        {
            return await IdentityWalletSDK.ExtrSignContent(tokenRequest.State, new ExtrSignContentRequest
            {
                Content = new ExtrSignContentData
                {
                    Data = new
                    {       
                        from = new
                        {
                            type = "address",
                            value = tokenRequest.FromAddress //"0xAeDc01B2245526F3Cae355688A31e67663405d04"
                        },
                        operation = new
                        {
                            type = "string",
                            value = "transfer"
                        },
                        to = new
                        {
                            type = "address",
                            value = tokenRequest.ToAddress //"0x9A9a48b9FF4d6F1E157f0cfa2C687a9947488B59"
                        },
                        token = new
                        {
                            type = "uint256",
                            value = tokenRequest.TokenId
                        }
                    }
                }
            });
        }                

        [HttpPost("decrypt-content")]
        public async Task<ActionResult> DecryptContent(DecryptContentRequest decryptContentRequest)
        {
            Console.WriteLine(decryptContentRequest.Content);

            var contentSigned = await IdentityWalletSDK.DecryptContent(decryptContentRequest.Content);

            var result = await APIWallet.VerifySignContent(contentSigned.Message,
                contentSigned.SignedContent.Signature, contentSigned.VerificationMethod);

            var capabilityDelegation = await APIWallet.VerifySignContent(contentSigned.Message, contentSigned.SignedContent.Signature, contentSigned.VerificationMethod, VerificationRelationship.CapabilityDelegation);
            Console.WriteLine($"CapabilityDelegation Verification Result: {capabilityDelegation}"); //Debe dar false ya que no está la entrada en el DID Document


            var auth = await APIWallet.VerifySignContent(contentSigned.Message, contentSigned.SignedContent.Signature, contentSigned.VerificationMethod, VerificationRelationship.Authentication);
            Console.WriteLine($"Authentication Verification Result: {auth}"); //Debe dar true ya que existe la entrada en el DID Document

            if (result)
            {
                Console.WriteLine(
                    $"r: {contentSigned.SignedContent.R} {Environment.NewLine}" +
                    $"s: {contentSigned.SignedContent.S} {Environment.NewLine}" +
                    $"v: {contentSigned.SignedContent.V} {Environment.NewLine}" +
                    $"Signature: {contentSigned.SignedContent.Signature} {Environment.NewLine}" +
                    $"Message: {contentSigned.Message} {Environment.NewLine}" +
                    $"Verification Method: {contentSigned.VerificationMethod} {Environment.NewLine}"
                );
            }
            else
            {
                throw new Exception("Ha ocurrido un error al validar la firma.");
            }

            return Ok();
        }

        [HttpPost("process-signature")]
        public async Task<ActionResult> ProcessSignature(ExtrSignedContent contentSigned)
        {
            Console.WriteLine(
                $"r: {contentSigned.SignedContent.R} {Environment.NewLine}" +
                $"s: {contentSigned.SignedContent.S} {Environment.NewLine}" +
                $"v: {contentSigned.SignedContent.V} {Environment.NewLine}" +
                $"Signature: {contentSigned.SignedContent.Signature} {Environment.NewLine}" +
                $"Message: {contentSigned.Message} {Environment.NewLine}" +
                $"Verification Method: {contentSigned.VerificationMethod} {Environment.NewLine}"
            );

            Console.WriteLine("Result: " + await APIWallet.VerifySignContent(contentSigned.Message, contentSigned.SignedContent.Signature, contentSigned.VerificationMethod));



            //Consumir Smart Contract con la firma

            return Ok();
        }
    }
}
