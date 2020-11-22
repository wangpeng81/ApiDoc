using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ApiDoc.Controllers
{
    public class FilesController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public Dictionary<string, object> UpLoad()
        {
            string auth = "";
            Dictionary<string, object> dict = new Dictionary<string, object>(); 
            foreach (KeyValuePair<string, StringValues> kv in this.Request.Form)
            {
                if (kv.Key == "__RequestVerificationToken")
                {
                    continue;
                }
                if (!dict.ContainsKey(kv.Key))
                {
                    if (auth != "")
                    {
                        auth += "_";
                    }
                    auth += kv.Key;
                    dict.Add(kv.Key, kv.Value[0]);
                }
            }

            if (this.Request.Form.Files.Count > 0)
            {
                IFormFile formFile = this.Request.Form.Files[0];
                var target = new System.IO.MemoryStream();
                formFile.CopyTo(target);

                byte[] buffer = target.ToArray(); 
                dict.Add("File", buffer);
            }

            //using (var stream = System.IO.File.Create(@"d:\\" + request.File.FileName))
            //{
            //    request.File.CopyTo(stream);
            //}
            return dict;
        }

    }
}
