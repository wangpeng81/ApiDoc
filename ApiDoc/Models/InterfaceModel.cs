using ApiDoc.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    [Table("api_interface")]
    public class InterfaceModel : BaseModel
    { 
        public string Title { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public int FKSN { get; set; } 
        public string ReturnType { get; set; }  
    }
}
