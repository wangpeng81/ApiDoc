using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.Models.Components;
using ApiDoc.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiDoc.Controllers
{
    public class MyConfigController : BaseController
    {
        private readonly MyConfig myConfig; 
        private JsonFileHelper jsonFileHelper;

        public MyConfigController(IHostEnvironment hostingEnvironment, 
            MyConfig myConfig  )
        {
            string path = hostingEnvironment.ContentRootPath + "\\MyConfig"; 
            jsonFileHelper = new JsonFileHelper(path);
            this.myConfig = myConfig; 
        }
        public IActionResult Index()
        {
            ViewData["Nav"] = base.LoadNav("MyConfig"); 
            MyConfig myConfig1 = jsonFileHelper.Read<MyConfig>(); 
            return View(myConfig1);
        }

        public int SaveAuthorize(string Authorize)
        { 
            jsonFileHelper.Write<string>("Authorize", Authorize);
            myConfig.Authorize = Authorize;
            return 100;
        }
    }
}
