using Telerik.Windows.Controls;

namespace AlohaFly.Interface
{
    public interface IMenuItem
    {
        DelegateCommand MenuAction();
    }
}
