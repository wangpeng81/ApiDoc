using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Utility.Filter
{
    public class CustomExceptionFilterAttribute: ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger )
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                string msg = $"{context.HttpContext.Request.Path} {context.Exception.Message}";
                this.logger.LogError(msg);
                context.Result = new JsonResult(new
                {
                    Result = false,
                    Msg = msg
                });
                context.ExceptionHandled = true;
            }
        }
    }
}
