using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Utility.Filter
{
    public class CustomExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                context.Result = new JsonResult(new
                {
                    Result = false,
                    Msg =  context.Exception.Message
                });
                context.ExceptionHandled = true;
            }
        }
    }
}
