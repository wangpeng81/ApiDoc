using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ApiDoc.Models.Responses
{
    //[Serializable]
    public class DSDataResult : BaseResult
    { 
        private DataSet result; 
        public DataSet Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
