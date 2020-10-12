using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Components
{
    public class NavItem
    {
        private string activeColor = "#8cbe00;";　
        public string ActiveColor { get => activeColor; set => activeColor = value; }

        public string Controller { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }
    }
}
