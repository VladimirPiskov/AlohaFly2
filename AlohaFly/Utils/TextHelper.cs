using Telerik.Windows.Documents.FormatProviders.Txt;
using Telerik.Windows.Documents.FormatProviders.Xaml;

namespace AlohaFly.Utils
{
    public static class TextHelper
    {
        public static string GetTextFromRadDocText(string RadDoc)
        {
            if (RadDoc == null || RadDoc.Trim() == "") return "";
            var d = new XamlFormatProvider().Import(RadDoc);
            var pr = new TxtFormatProvider();
            return pr.Export(d);
        }
    }
}
