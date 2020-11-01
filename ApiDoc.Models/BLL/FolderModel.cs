using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Threading.Tasks;
using ApiDoc.Models.Attributes;

namespace ApiDoc.Models
{
    
    public class FolderModel : BaseModel
    { 
        public string FolderName { get; set; }
        public int ParentSN { get; set; }
        public string RoutePath { get; set; } //路由路径
    }
}
