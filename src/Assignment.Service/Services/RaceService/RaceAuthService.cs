using Assignment.Api.Interfaces.RaceInterface;
using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assignment.Service.Model;

namespace Assignment.Service.Services.RaceService
{
    public class RaceAuthService
    {
        private readonly IDBRaceUserRepository _raceUserRepository;

        public RaceAuthService(IDBRaceUserRepository dBRaceUserRepository)
        {
            _raceUserRepository = dBRaceUserRepository;
        }
        public async Task<int> AuthenticateAsync(AuthRQ authRequest)
        {
            var ret = await _raceUserRepository.AuthenticateAsync(authRequest.Email, authRequest.Password);
            return ret;
        }
        public async Task<string> GenerateF1AdminJweToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("Subject")),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };
            var NullList = new List<Claim>();
            claims.Add(new Claim("email", email));
            claims.Add(new Claim("permission", "race::manage"));
            var token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("ValidIssuer"),
                Environment.GetEnvironmentVariable("ValidIssuer"),
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );


            var encryptedJwt = EncryptJwt(token);

            return encryptedJwt;
        }
        public string EncryptJwt(JwtSecurityToken token)
        {
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var payloadJson = JsonConvert.SerializeObject(tokenString);
            var certificatePath = Environment.GetEnvironmentVariable("CertificatePath");

            X509Certificate2 certWithPublicKey = new X509Certificate2(certificatePath);
            RSA rsaPublicKey = certWithPublicKey.GetRSAPublicKey();

            string encryptedJwt = JWT.Encode(payloadJson, rsaPublicKey, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);

            return encryptedJwt;
        }
    }
}
