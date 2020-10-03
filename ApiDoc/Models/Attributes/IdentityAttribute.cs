using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentityAttribute : Attribute
    {
        /// <summary> 
        /// true:是; false:否 
        /// </summary>

        public bool IsIdentity { get; set; }
    }
}
