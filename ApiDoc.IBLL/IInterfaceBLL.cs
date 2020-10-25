using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.IBLL
{
    public interface IInterfaceBLL : IBaseBLL
    {
        DBInterfaceModel GetInterfaceModel(int SN); 
    }
}
