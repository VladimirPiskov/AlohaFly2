using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Telerik.Windows.Documents.FormatProviders;
using Telerik.Windows.Documents.FormatProviders.Xaml;
using Telerik.Windows.Documents.Model;

namespace AlohaFly.Models
{
    public class PrintOrderKitchenToFly : ViewModelPane
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private const string SampleDocumentPath = "data/KitchenOrderTemplate.xaml";
        public PrintOrderKitchenToFly(OrderFlight _order)
        {
            order = _order;
            LoadTemplate();
            ApplyOrderToTemplate();
        }



        public RadDocument Document
        {
            get
            {
                return document;
            }
        }


        public string DocumentXalmText
        {
            get
            {
                XamlFormatProvider provider = new XamlFormatProvider();
                return provider.Export(document);
                //return document.ToString();
            }
        }

        private void ApplyOrderToTemplate()
        {
            try
            {
                if (document == null || document.Sections == null || document.Sections.Count < 1) return;
                var S1 = document.Sections.First();

                var PCompanyName = S1.Blocks.Where(a => (a.Children != null && a.Children.First != null && a.Children.First.ToString().Contains("Компания"))).First();
                (PCompanyName.Children.First() as Span).Text = $"Компания {order.AirCompany.Name}";

                SetSpanText("Компания", $"Компания: {order.AirCompany.Name}");
                SetSpanText("Борт", $"Борт: {order.FlightNumber}");
                SetSpanText("Доставка", $"Доставка: {order.DeliveryPlace?.Name}");
                SetSpanText("Дата", $"Дата: {order.DeliveryDate.ToString("dd.MM.yyyy HH:mm")}");
                SetSpanText("Контакты", $"Контакты: {order.ContactPerson?.FullSearchData}");
                SetSpanText("Машина забирает", $"Машина забирает: {order.ExportTime.ToString("HH:mm")}");
                SetSpanText("Готовность в ресторане", $"Готовность в ресторане в {order.ReadyTime.ToString("HH:mm")}");
                //var orderFlightSpan = GetSpan("Перелет");
                Paragraph passageNumberParagraph = GetParagraph("Перелет");
                document.Sections.First().Blocks.Remove(passageNumberParagraph);
                passageNumberParagraph.FontSize = 26.6666667;
                passageNumberParagraph.SpacingAfter = 0;
                passageNumberParagraph.TextAlignment = Telerik.Windows.Documents.Layout.RadTextAlignment.Center;
                Paragraph dishParagraph = GetParagraph("Классический салат из рукколы");
                document.Sections.First().Blocks.Remove(dishParagraph);
                Paragraph emptyStringParagraph = new Paragraph(dishParagraph);
                emptyStringParagraph.Children.Add(new Span(""));

                foreach (var fn in order.DishPackagesNoSpis.Select(a => ((DishPackageFlightOrder)a).PassageNumber).Distinct().OrderBy(a => a))
                {

                    if (order.DishPackagesNoSpis.Select(a => ((DishPackageFlightOrder)a).PassageNumber).Distinct().Count() > 1)
                    {
                        Paragraph p = new Paragraph();
                        p.FontSize = 26.6666667;
                        p.SpacingAfter = 0;
                        p.TextAlignment = Telerik.Windows.Documents.Layout.RadTextAlignment.Center;
                        p.Children.Add(new Span($"{fn}-й Перелет. Упаковать отдельно, подписать!!!")
                        {
                            FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                            FontSize = 26.6666666666667,
                            FontWeight = FontWeights.Bold
                        });

                        document.Sections.First().Children.Add(p);
                        document.Sections.First().Children.Add(new Paragraph(emptyStringParagraph));
                    }



                    foreach (var KGroup in order.DishPackagesNoSpis.Where(a => ((DishPackageFlightOrder)a).PassageNumber == fn && a.Dish.DishKitсhenGroupId != null).Select(a => a.Dish.DishKitсhenGroupId).Distinct().OrderBy(a => a.Value))
                    {

                        foreach (var Pak in order.DishPackagesNoSpis.Where(a => ((DishPackageFlightOrder)a).PassageNumber == fn && a.Dish.DishKitсhenGroupId != null && a.Dish.DishKitсhenGroupId == KGroup).OrderBy(a => a.PositionInOrder))
                        {
                            AddDishparagraph((DishPackageFlightOrder)Pak);
                        }
                        document.Sections.First().Children.Add(new Paragraph(emptyStringParagraph));
                    }

                    //var NPaks = order.DishPackages.Where(a => a.PassageNumber == fn && a.Dish.DishKitсhenGroupId == null);
                    foreach (var Pak in order.DishPackagesNoSpis.Where(a => ((DishPackageFlightOrder)a).PassageNumber == fn && a.Dish.DishKitсhenGroupId == null))
                    {
                        AddDishparagraph((DishPackageFlightOrder)Pak);

                    }
                    document.Sections.First().Children.Add(new Paragraph(emptyStringParagraph));
                }


            }
            catch (Exception e)
            {

            }

        }
        private void AddDishparagraph(DishPackageFlightOrder Pak)
        {
            string s = $"({Pak.Dish.Barcode}) {Pak.Dish.Name} - {Pak.Amount.ToString("0.##")} порц.";
            if (Pak.Comment.Trim() != "")
            {
                try
                {
                    /*
                    var d = new XamlFormatProvider().Import(Pak.Comment);
                    var pr = new TxtFormatProvider();
                    */
                    string Comment = Utils.TextHelper.GetTextFromRadDocText(Pak.Comment);
                    if (Comment.Trim() != "")
                    {
                        s += $" ({Comment})";
                    }
                }
                catch (Exception e)
                {
                    logger.Debug($"AddDishparagraph comment error {e.Message}");
                }
            }
            Paragraph p2 = new Paragraph();
            p2.FontSize = 16;
            p2.SpacingAfter = 0;
            p2.Children.Add(new Span(s)
            {
                FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                FontSize = 16,
            });
            document.Sections.First().Children.Add(p2);
        }

        private Paragraph GetParagraph(string spanMark)
        {
            try
            {
                var S1 = document.Sections.First();
                var PCompanyName = S1.Blocks.Where(a => (a.Children != null && a.Children.First != null && a.Children.First.ToString().Contains(spanMark))).First();
                return (PCompanyName as Paragraph);
            }
            catch
            { }
            return null;

        }


        private void SetSpanText(string oldSpanMark, string newText)
        {
            try
            {
                var S1 = document.Sections.First();
                var PCompanyName = S1.Blocks.Where(a => (a.Children != null && a.Children.First != null && a.Children.First.ToString().Contains(oldSpanMark))).First();
                (PCompanyName.Children.First() as Span).Text = newText;
            }
            catch
            { }

        }

        private void LoadTemplate()
        {
            using (Stream stream = Application.GetResourceStream(GetResourceUri(SampleDocumentPath)).Stream)
            {
                IDocumentFormatProvider xamlProvider = DocumentFormatProvidersManager.GetProviderByExtension(".xaml");
                document = xamlProvider.Import(stream);
            }


        }
        private static Uri GetResourceUri(string resource)
        {
            AssemblyName assemblyName = new AssemblyName(typeof(PrintOrderKitchenToFly).Assembly.FullName);
            string resourcePath = "/" + assemblyName.Name + ";component/" + resource;
            Uri resourceUri = new Uri(resourcePath, UriKind.Relative);

            return resourceUri;
        }

        RadDocument document;
        OrderFlight order { set; get; }
    }
}
