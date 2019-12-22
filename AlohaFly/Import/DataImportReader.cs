using Telerik.Windows.Controls;

namespace AlohaFly.Import
{
    abstract class DataImportReader : Interface.IMenuItem
    {
        public virtual DelegateCommand MenuAction()
        {
            return null;
        }


    }
}
