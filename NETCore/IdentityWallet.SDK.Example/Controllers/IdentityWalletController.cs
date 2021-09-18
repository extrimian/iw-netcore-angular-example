﻿using IdentityWallet.SDK.Models;
using IdentityWallet.SDK.Models.DIDCommMessages;
using IdentityWallet.SDK.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityWalletController : ControllerBase
    {
        private IdentityWalletSDK IdentityWalletSDK;

        const string DAPP_DID = "did:ethr:rsk:testnet:0xD17de0e4288920F1BCCbED78f387a1e5e37BAE10";
        const string IW_DID = "did:ethr:rsk:testnet:0xC2Cb8a25eD3F1f0d5ba2E506B0500fd8322aAF15";
        const string IW_VM = "did:ethr:rsk:testnet:0xC2Cb8a25eD3F1f0d5ba2E506B0500fd8322aAF15#delegate-1";

        const string API_WALLET_USERNAME = "dapp@anh.com";
        const string API_WALLET_PWD = "DappANH1234";

        public APIWallet APIWallet { get; set; }

        public IdentityWalletController()
        {
            IdentityWalletSDK = new IdentityWalletSDK(DAPP_DID, IW_DID, IW_VM, DIDCommPack, DIDCommUnpack, LoggedIn);

            APIWallet = new APIWallet(API_WALLET_USERNAME, API_WALLET_PWD);
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

        private Task LoggedIn(LoginVC loginVC, LoginCredentialSubject subject)
        {
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-AccessToken");
            HttpContext.Response.Headers.Add("X-AccessToken", GenerateToken(subject.name, subject.did));
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
            return await IdentityWalletSDK.Handshake(state, "es_ES");
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

        [HttpPost("extr-sign-content")]
        public async Task<ActionResult<SDKCommunicationMessage>> ExtrSignContent(SDKOperationInstance state)
        {
            return await IdentityWalletSDK.ExtrSignContent(state, new ExtrSignContentRequest
            {
                TemplateId = "change-owner",
                Content = new ExtrSignContentData
                {
                    FriendlyContent = "Cambiar la propiedad de la identidad 0xD9001096218e4b7c30ec1B7C85A61AfAe3e450aD " +
                                       "al address 0x89205A3A3b2A69De6Dbf7f01ED13B2108B2c43e7",
                    FriendlyContentOrder = 5,
                    Data = new
                    {
                        header1 = new
                        {
                            type = "bytes1",
                            value = "0x19",
                        },
                        header2 = new
                        {
                            type = "bytes1",
                            value = "0x00"
                        },
                        registry = new
                        {
                            type = "address",
                            value = "0x7EEb5772eF87C40255a74C7cC05317C08eA64214"
                        },
                        registryNonce = new
                        {
                            type = "uint256",
                            value = 0
                        },
                        identity = new
                        {
                            type = "address",
                            value = "0xD9001096218e4b7c30ec1B7C85A61AfAe3e450aD"
                        },
                        operation = new
                        {
                            type = "string",
                            value = "changeOwner"
                        },
                        newOwner = new
                        {
                            type = "address",
                            value = "0x89205A3A3b2A69De6Dbf7f01ED13B2108B2c43e7"
                        }
                    }
                }
            });
        }

        [HttpPost("process-signature")]
        public ActionResult ProcessSignature(ExtrContentSigned contentSigned)
        {
            Console.WriteLine(
                $"r: {contentSigned.ContentSigned.R} {Environment.NewLine}" +
                $"s: {contentSigned.ContentSigned.S} {Environment.NewLine}" +
                $"v: {contentSigned.ContentSigned.V} {Environment.NewLine}" +
                $"Signature: {contentSigned.ContentSigned.Signature} {Environment.NewLine}" +
                $"Message: {contentSigned.Message} {Environment.NewLine}" +
                $"Verification Method: {contentSigned.VerificationMethod} {Environment.NewLine}"
            );

            //Consumir Smart Contract con la firma

            return Ok();
        }
    }
}
