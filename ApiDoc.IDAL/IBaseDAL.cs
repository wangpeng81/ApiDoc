using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.IDAL
{
    public interface IBaseDAL
    {
        int Insert(BaseModel model);
        int Update(BaseModel model);
        int Delete(BaseModel model);

        T Get<T>(int SN) where T : class, new();
    }
}
