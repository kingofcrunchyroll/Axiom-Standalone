using System;
using System.Collections.Generic;
using System.Linq;

namespace Axiom.Classes
{
    public class ButtonInfo
    {
        public string buttonText = "-";
        public string overlapText = null;
        public Action method = null;
        public Action postMethod = null;
        public Action fixedMethod = null;
        public bool label = false;
        public Action enableMethod = null;
        public Action disableMethod = null;
        public bool enabled = false;
        public bool isTogglable = true;
        public bool incremental = false;
        public string toolTip = "This button doesn't have a tooltip/tutorial.";
    }
    public class Category
    {
        public string Name = "Placeholder";
        public string Icon = "missingtexture";
        public Category ParentCategory = null;
        public List<ButtonInfo> Buttons = new List<ButtonInfo>();
        public List<Category> Subcategories = new List<Category>();
        public Category Add(Category sub)
        {
            sub.ParentCategory = this;
            this.Subcategories.Add(sub);
            return this;
        }
        public ButtonInfo GetButton(string name)
        {
            return this.Buttons.FirstOrDefault(b => b.buttonText == name);
        }

        public Category GetSubcategory(string name)
        {
            return Subcategories.FirstOrDefault(c => c.Name == name);
        }
    }
    public class SubCategory : Category { }
}
