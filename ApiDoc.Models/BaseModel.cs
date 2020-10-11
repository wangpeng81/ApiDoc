using ApiDoc.Models.Attributes;
using System; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace ApiDoc.Models
{
    public class BaseModel
    {
        [Identity(IsIdentity = true)]
        public int SN { get; set; }
 
    }
}
