using System.Collections.ObjectModel;

namespace AlohaFly.Models
{

    public class MenuItemsCollection : ObservableCollection<MenuItem>
    {
        private MenuItem parent;

        public MenuItemsCollection() : this(null)
        {
        }

        public MenuItemsCollection(MenuItem parent)
        {
            this.parent = parent;
        }

        public MenuItem Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        protected override void InsertItem(int index, MenuItem item)
        {
            item.Parent = this.Parent;
            base.InsertItem(index, item);
        }
    }
}

