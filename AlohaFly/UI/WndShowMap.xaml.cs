using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndShowMap.xaml
    /// </summary>
    public partial class WndShowMap :  RadWindow
    {
        public WndShowMap()
        {
            InitializeComponent();
        }
        /*
        public void SetUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                url = @"https://yandex.ru/maps/print/?mode=search&text=%D1%81%D0%B0%D0%B4%D0%BE%D0%B2%D0%B0%D1%8F%20%D0%BA%D0%B0%D1%80%D0%B5%D1%82%D0%BD%D0%B0%D1%8F%20%D1%83%D0%BB%D0%B8%D1%86%D0%B0%20%D0%B4%D0%BE%D0%BC%202%2024&z=17";
            }
            Browser.Address = url;
            //Browser.Load(url);
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            SetUrl("yandex.ru");
        }
        */
    }
}
