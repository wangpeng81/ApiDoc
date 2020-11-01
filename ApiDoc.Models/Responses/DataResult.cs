using System;
using System.Data;

namespace ApiDoc.Models.Responses
{ 
    public class DataResult : BaseResult
    {

        private object result;
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

    }
}
