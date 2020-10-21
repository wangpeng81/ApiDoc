
using ApiDoc.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiDoc.IDAL
{
    public interface IInterfaceDAL : IBaseDAL
    {
        List<InterfaceModel> All();
        List<InterfaceModel> All(string title, string url, int fksn);
        List<InterfaceModel> Query(bool isStop);
        string FullPath(int fksn);

    }
}
