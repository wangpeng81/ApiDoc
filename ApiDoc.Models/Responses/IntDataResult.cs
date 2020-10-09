using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Responses
{ 
    public class IntDataResult : DataResult
    {
        public IntDataResult()
        {

        }
        private int result = 0;
        public new int Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
