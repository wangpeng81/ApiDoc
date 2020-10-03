using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Threading.Tasks;
using ApiDoc.Models.Attributes;

namespace ApiDoc.Models
{
    [Table("api_folder")] 
    public class FolderModel : BaseModel
    { 
        public string FolderName { get; set; }
        public int ParentSN { get; set; }  
    }
}
