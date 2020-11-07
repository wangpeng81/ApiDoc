using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.AuthenticationCenter;
using ApiDoc.Middleware;
using ApiDoc.Models.BLL;
using ApiDoc.Models.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiDoc.Controllers
{
    [Route("[controller]")] 
    public class AuthorController : BaseController
    {
        private readonly DBRouteValueDictionary routeDict;
        private readonly MyConfig myConfig;
        private readonly IJWTService iJWTService;
        
        public AuthorController(DBRouteValueDictionary _routeDict, MyConfig myConfig, 
             IJWTService iJWTService )
        {
            routeDict = _routeDict;
            this.myConfig = myConfig;
            this.iJWTService = iJWTService; 
        }

        [Route("Index")]
        [Authorize]
        public string Index()
        {
            return this.myConfig.Authorize;
        }

        [Route("Login")]
        [HttpPost]
        public string Login([FromBody] UserModel userModel)
        {
            string token = this.iJWTService.GetToken(userModel.UserName, userModel.Password);
            return JsonConvert.SerializeObject(new
            {
                result = true,
                token
            }); 
        }

        //[AllowAnonymous]
        //[HttpPost, Route("requestToken")]
        //public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("Invalid Request");
        //    }

        //    string token;
        //    if (authService.IsAuthenticated(request, out token))
        //    {
        //        return Ok(token);
        //    }

        //    return BadRequest("Invalid Request");
        
        //}
    }
}

