using System;
using System.Windows.Input;
using System.Windows.Markup;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    [ContentProperty("Items")]
    public class MenuViewModel : ViewModelBase
    {
        private MenuItemsCollection _items;
        private Uri _source;
        private DelegateCommand notifyOnClickCommand;
        private string notifyTextBlockText;

        public MenuViewModel()
        {
            this._items = new MenuItemsCollection();

            //The command is used in the Configuration example for notifying when element with children has been clicked.
            this.notifyOnClickCommand = new DelegateCommand(this.NotifyOnClickExecuted);
        }

        public MenuItemsCollection Items
        {
            get
            {
                return this._items;
            }
        }

        public Uri Source
        {
            get
            {
                return this._source;
            }
            set
            {
                this._source = value;
                this.CreateMenuItems();
            }
        }

        public ICommand NotifyOnClickCommand
        {
            get
            {
                return this.notifyOnClickCommand;
            }
        }

        public void NotifyOnClickExecuted(object parameter)
        {
            var menuItem = (MenuItem)parameter;
            if (menuItem.Items.Count != 0)
            {
                this.NotifyTextBlockText = string.Format(menuItem.Text + " has been clicked");
            }
        }

        public string NotifyTextBlockText
        {
            get
            {
                return this.notifyTextBlockText;
            }
            set
            {
                this.notifyTextBlockText = value;
                this.OnPropertyChanged("NotifyTextBlockText");
            }
        }

        private void CreateMenuItems()
        {

            /* Instead of loading a XML document, you could query a web service to get the menu items */
            /*
            if (this.Source == null) return;

            using (StreamReader reader = new StreamReader(
                Application.GetResourceStream(this.Source).Stream))
            {
                string xaml = reader.ReadToEnd();

                this._items = ParseXaml(xaml);
            }
            */
        }

        private MenuItemsCollection ParseXaml(string xaml)
        {
            return XamlReader.Parse(xaml) as MenuItemsCollection;
        }
    }
}
