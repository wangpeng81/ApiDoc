using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.Middleware;
using ApiDoc.Models.Components;
using Microsoft.AspNetCore.Mvc;

namespace ApiDoc.Controllers
{
    public class AuthorController : Controller
    {
        private readonly DBRouteValueDictionary routeDict;
        private readonly MyConfig myConfig;

        public AuthorController(DBRouteValueDictionary _routeDict, MyConfig myConfig)
        {
            routeDict = _routeDict;
            this.myConfig = myConfig;
        }
        public string Index()
        {
            return this.myConfig.Authorize;
        }
    }
}
