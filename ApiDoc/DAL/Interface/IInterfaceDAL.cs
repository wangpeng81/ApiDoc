
using ApiDoc.Models;
using System.Collections.Generic;

namespace ApiDoc.DAL
{
    public interface IInterfaceDAL : IBaseDAL
    {
        List<InterfaceModel> All();
        List<InterfaceModel> All(string title, string url, int fksn);
    }
}
