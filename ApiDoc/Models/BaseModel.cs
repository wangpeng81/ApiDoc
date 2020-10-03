using ApiDoc.Models.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    public class BaseModel
    {
        [Identity(IsIdentity = true)]
        public int SN { get; set; }
  
    }
}
