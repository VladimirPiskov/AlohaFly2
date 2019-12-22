using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlohaFly.DataExtension
{
    public class AirCopmanyVis : AlohaService.ServiceDataContracts.AirCompany
    {
        public AirCopmanyVis(AlohaService.ServiceDataContracts.AirCompany Parent) : base()
        {
            base.Address = Parent.Address;
        }



        [DisplayName("Адрес")]
        public new string Address { get { return base.Address; } }


    }
    public static class DataVisualExtension
    {
        public static List<DataGridColumn> GetDataColumns()
        {
            var res = new List<DataGridColumn>();
            var col1 = new DataGridTextColumn() { Header = "Адрес" };
            var bind = new Binding();
            bind.Path = new PropertyPath("Address");
            col1.Binding = bind;

            res.Add(col1);
            return res;

        }
        public static void SetAttrs(AirCopmanyVis cmp)
        {
            PropertyInfo propertyInfo = typeof(AirCopmanyVis).GetProperty("Address");
            DefaultValueAttribute attribut = (DefaultValueAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(DefaultValueAttribute));
            //DefaultValueAttribute attribut = (DefaultValueAttribute)Attribute.
        }

    }

    /*
        public static class DataExtension
    {
        
    }
    */
}
