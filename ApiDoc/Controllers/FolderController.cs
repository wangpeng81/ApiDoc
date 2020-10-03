using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.DAL;
using ApiDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiDoc.Controllers
{
    public class FolderController : Controller
    { 
        private readonly IFolderDAL folderDAL; 
        public FolderController(IFolderDAL folderDAL)
        { 
            this.folderDAL = folderDAL;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public FolderModel SaveFolder([FromQuery] FolderModel info)
        { 
            if (info.SN > 0)
            {
                this.folderDAL.Update(info);
            }
            else
            {
                info.SN =  this.folderDAL.Insert(info);
            }
            return info;
        }

        [HttpGet]
        public List<TreeViewItem> All(bool root)
        {
            List<TreeViewItem> list = new List<TreeViewItem>();
            if (root)
            {
                List<TreeViewItem> rootlist = new List<TreeViewItem>();

                TreeViewItem tviRoot = new TreeViewItem();
                tviRoot.href = "";
                tviRoot.text = "全部";
                tviRoot.data = new FolderModel();
                tviRoot.nodes = rootlist;
                list.Add(tviRoot);

                List<TreeViewItem> listChild = this.folderDAL.All();
                foreach (TreeViewItem tvi in listChild)
                {
                    rootlist.Add(tvi);
                }
            }
            else
            {
                list = this.folderDAL.All();
            }
            
            return list; 
        }
 
        [HttpGet]
        public int DeleteFolder(int SN)
        {
            FolderModel model = new FolderModel();
            model.SN = SN;
            int result = this.folderDAL.Delete(model);
            return result;
        }
    }
}
