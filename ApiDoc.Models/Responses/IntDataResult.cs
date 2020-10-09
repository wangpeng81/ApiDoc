using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Responses
{
    [Serializable]
    public class IntDataResult : DataResult
    {
        private int result = 0;
        public new int Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
