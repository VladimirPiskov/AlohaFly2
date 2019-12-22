using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Aloha.Alerts
{
    public class CategoryToBrushConverter : IValueConverter
    {
        private string _categoryName;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is CheckBoxItem)
            {
                _categoryName = (value as CheckBoxItem).Text;
            }
            else
            {
                _categoryName = value.ToString();
            }

            switch (_categoryName)
            {
                case "Company":
                    return new SolidColorBrush(Color.FromArgb(255, 148, 176, 243));
                case "Team":
                    return new SolidColorBrush(Color.FromArgb(255, 255, 239, 191));
                default:
                    return new SolidColorBrush(Color.FromArgb(255, 165, 211, 211));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
