using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.BLL
{
    public interface IBaseBLL
    {
        int Add(BaseModel model);
        int Update(BaseModel model);
        int Delete(BaseModel model); 
    }
}
