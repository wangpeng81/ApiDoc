using ApiDoc.Models.BLL;
using ApiDoc.Models.Components;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiDoc.AuthenticationCenter
{
    public class JWTHSService : IJWTService
    {
        private readonly JWTTokenOptions jwtTokenOptions;

        public JWTHSService(MyConfig myConfig)
        { 
            this.jwtTokenOptions = myConfig.JWTTokenOptions;
        }

        public string GetToken(string UserName, string Password)
        {
            var claims = new[]  //这里就是有效载荷
           {
                   new Claim(ClaimTypes.Name, UserName), 
                   new Claim("UserPassword",Password)  
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtTokenOptions.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            /**
             * Claims (Payload)
                Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:

                iss: The issuer of the token，token 是给谁的
                sub: The subject of the token，token 主题
                exp: Expiration Time。 token 过期时间，Unix 时间戳格式
                iat: Issued At。 token 创建时间， Unix 时间戳格式
                jti: JWT ID。针对当前 token 的唯一标识
                除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
             * */
            var token = new JwtSecurityToken(
                issuer: this.jwtTokenOptions.Issuer,
                audience: this.jwtTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),//5分钟有效期
                //notBefore: DateTime.Now,//0分钟后有效
                signingCredentials: creds);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }
    }
}
