using ApiDoc.Models.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Controllers
{
    public class BaseController : Controller
    {
        protected List<NavItem> LoadNav(string text)
        {
            List<NavItem> listNav = new List<NavItem>();

            NavItem item = new NavItem();
            item.ActiveColor = "#6c757d;";
            item.Controller = "Home";
            item.Icon = "fa-home";
            item.Text = "首页";  
            listNav.Add(item);

            item = new NavItem();
            item.ActiveColor = "#6c757d;";
            item.Controller = "Folder";
            item.Icon = "fa-folder";
            item.Text = "目录";
            listNav.Add(item);

            item = new NavItem();
            item.ActiveColor = "#6c757d;";
            item.Controller = "MyConfig";
            item.Icon = "fa-cog";
            item.Text = "配置";
            listNav.Add(item);

            foreach (NavItem item1 in listNav)
            {
                if (item1.Controller == text)
                {
                    item1.ActiveColor = "#8cbe00";
                }
            }
            return listNav;
        }
    }
}
