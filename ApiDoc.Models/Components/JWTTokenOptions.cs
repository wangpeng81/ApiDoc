using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Components
{
    public class JWTTokenOptions
    {
        public string Audience
        {
            get;
            set;
        }

        public string SecurityKey
        {
            get;
            set;
        }

        public string Issuer
        {
            get;
            set;
        }
    }
}
