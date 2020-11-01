using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Responses
{ 
    public class IntDataResult : BaseResult
    {  
        private int result = 0;
        public int Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
