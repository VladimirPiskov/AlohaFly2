using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AlohaFly.Utils
{
    public class EditableTextBox
    {
        public static int GetCharacterCasing(DependencyObject obj)
        {
            return (int)obj.GetValue(CharacterCasingProperty);
        }
        public static void SetCharacterCasing(DependencyObject obj, int value)
        {
            obj.SetValue(CharacterCasingProperty, value);
        }
        public static readonly DependencyProperty CharacterCasingProperty =
            DependencyProperty.RegisterAttached("CharacterCasing", typeof(int), typeof(EditableTextBox), new UIPropertyMetadata(OnCharacterCasingChanged));
        private static void OnCharacterCasingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = obj as Telerik.Windows.Controls.RadAutoCompleteBox;
            if (comboBox == null)
            {
                return;
            }
            comboBox.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (DispatcherOperationCallback)delegate
                {
                    var childrenCount = VisualTreeHelper.GetChildrenCount(comboBox);
                    if (childrenCount > 0)
                    {
                        var rootElement = VisualTreeHelper.GetChild(comboBox, 0) as FrameworkElement;
                        TextBox textBox = (TextBox)rootElement.FindName("PART_EditableTextBox");
                        if (textBox != null)
                            textBox.SetValue(TextBox.CharacterCasingProperty, (CharacterCasing)e.NewValue);
                    }
                    return null;
                }
                , null);
        }
    }
}
