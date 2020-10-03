﻿using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL
{
    public interface IFolderDAL : IBaseDAL
    {
        List<TreeViewItem> All();
    }
}
