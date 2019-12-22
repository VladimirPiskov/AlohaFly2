using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    [ContentProperty("Items")]
    public class MenuItem : INotifyPropertyChanged
    {
        private bool isChecked;
        private bool isEnabled = true;
        private string text;
        private string groupName;
        private bool isCheckable;
        private bool isSeparator;
        private string imageUrl;
        private bool staysOpenOnClick;
        private MenuItemsCollection items;
        private MenuItem parent;

        public MenuItem()
        {
            this.items = new MenuItemsCollection(this);
        }

        public MenuItem(string text)
        {
            Text = text;
            this.items = new MenuItemsCollection(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private DelegateCommand _Command;
        public ICommand Command
        {
            get
            {
                return this._Command;
            }
            set
            {
                _Command = (DelegateCommand)value;
            }
        }


        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public string GroupName
        {
            get
            {
                return this.groupName;
            }
            set
            {
                this.groupName = value;
            }
        }

        public bool IsCheckable
        {
            get
            {
                return this.isCheckable;
            }
            set
            {
                this.isCheckable = value;
            }
        }

        public bool IsSeparator
        {
            get
            {
                return this.isSeparator;
            }
            set
            {
                this.isSeparator = value;
            }
        }

        public string ImageUrl
        {
            get
            {
                return this.imageUrl;
            }
            set
            {
                this.imageUrl = value;
            }
        }

        public bool StaysOpenOnClick
        {
            get
            {
                return this.staysOpenOnClick;
            }
            set
            {
                this.staysOpenOnClick = value;
            }
        }

        public MenuItemsCollection Items
        {
            get
            {
                return this.items;
            }
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

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (value != this.isChecked)
                {
                    this.isChecked = value;
                    this.OnPropertyChanged("IsChecked");

                    if (!string.IsNullOrEmpty(this.GroupName))
                    {
                        if (this.IsChecked)
                        {
                            this.UncheckOtherItemsInGroup();
                        }
                        else
                        {
                            this.IsChecked = true;
                        }
                    }
                }
            }
        }

        public Image Image
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImageUrl)) return null;

                return new Image()
                {
                    Source = new BitmapImage(new Uri(this.ImageUrl, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.None
                };
            }
        }

        private void UncheckOtherItemsInGroup()
        {
            IEnumerable<MenuItem> groupItems = this.Parent.Items.Where((MenuItem item) => item.GroupName == this.GroupName);
            foreach (MenuItem item in groupItems)
            {
                if (item != this)
                {
                    item.isChecked = false;
                    item.OnPropertyChanged("IsChecked");
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (null != this.PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
