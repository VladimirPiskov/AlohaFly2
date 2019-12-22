using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace Aloha.Alerts
{
    public class OrientedGroupHeaderContentTemplateSelector : ScheduleViewDataTemplateSelector
    {
        private DataTemplate _horizontalTemplate;
        private DataTemplate _verticalTemplate;
        private DataTemplate _horizontalResourceTemplate;
        private DataTemplate _verticalResourceTemplate;

        public DataTemplate HorizontalTemplate
        {
            get
            {
                return this._horizontalTemplate;
            }
            set
            {
                this._horizontalTemplate = value;
            }
        }

        public DataTemplate VerticalTemplate
        {
            get
            {
                return this._verticalTemplate;
            }
            set
            {
                this._verticalTemplate = value;
            }
        }

        public DataTemplate HorizontalResourceTemplate
        {
            get
            {
                return this._horizontalResourceTemplate;
            }
            set
            {
                this._horizontalResourceTemplate = value;
            }
        }

        public DataTemplate VerticalResourceTemplate
        {
            get
            {
                return this._verticalResourceTemplate;
            }
            set
            {
                this._verticalResourceTemplate = value;
            }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container, ViewDefinitionBase activeViewDeifinition)
        {
            CollectionViewGroup cvg = item as CollectionViewGroup;
            if (cvg != null && cvg.Name is IResource)
            {
                if (activeViewDeifinition.GetOrientation() == Orientation.Vertical)
                {
                    if (this.HorizontalResourceTemplate != null)
                    {
                        return this.HorizontalResourceTemplate;
                    }
                }
                else
                {
                    if (this.VerticalResourceTemplate != null)
                    {
                        return this.VerticalResourceTemplate;
                    }
                }
            }

            if (cvg != null && cvg.Name is DateTime)
            {
                if (activeViewDeifinition.GetOrientation() == Orientation.Vertical)
                {
                    if (this.HorizontalTemplate != null)
                    {
                        return this.HorizontalTemplate;
                    }
                }
                else
                {
                    if (this.VerticalTemplate != null)
                    {
                        return this.VerticalTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container, activeViewDeifinition);
        }
    }
}
