using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;
namespace AlohaFly.Reports
{
    public class WordReports
    {
        //private const string SampleDocumentPath = "data/KitchenOrderTemplate.xaml";
        private const string SampleDocumentPath = @"\data\KitchenOrderTemplate.dotx";

        Word.Document document = null;
        Logger logger = LogManager.GetCurrentClassLogger();
        public WordReports()
        { }



        public void PrintKitchenDocumentToGo(OrderToGo order)
        {
            if (order == null) return;
            try
            {
                logger.Debug($"PrintKitchenDocumentToGo {order.Id}");
                // Создаём объект приложения
                Word.Application app = new Word.Application();
                app.Visible = true;
                // Путь до шаблона документа
                // Открываем

                document = app.Documents.Add(System.AppDomain.CurrentDomain.BaseDirectory + SampleDocumentPath);
                document.Paragraphs.Add();
                int ParNum = 1;
                AddLeftCaptionText(document.Paragraphs[1], $"Заказ {order.Id}", true);
                document.Paragraphs.Add();

                AddLeftCaptionText(document.Paragraphs.Last, $"Клиент {order.OrderCustomer?.FullName}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Телефон {order.PhoneNumber}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Адрес {order.Address.AddressExt}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Дата {order.DeliveryDate.ToString("dd.MM.yyyy HH:mm")}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Машина забирает: {order.ExportTime.ToString("HH:mm")}");
                document.Paragraphs.Add();
                AddMiddleCaptionText(document.Paragraphs.Last, $"Готовность в ресторане в {order.ReadyTime.ToString("HH:mm")}", true);
                string comment = order.CommentKitchen;
                if (comment != null && comment != "")
                {
                    document.Paragraphs.Add();
                    AddMiddleCaptionText(document.Paragraphs.Last, comment);
                }




                foreach (var KGroup in order.DishPackagesNoSpis.Select(a => a.Dish.DishKitсhenGroupId).Distinct().OrderBy(a => a.Value))
                {

                    foreach (var Pak in order.DishPackagesNoSpis.Where(a => ((a.Dish.DishKitсhenGroupId != null && a.Dish.DishKitсhenGroupId == KGroup))).OrderBy(a => a.PositionInOrder))
                    {
                        document.Paragraphs.Add();
                        AddDishparagraph(document.Paragraphs.Last, (DishPackageToGoOrder)Pak);
                    }
                    document.Paragraphs.Add();
                    AddDishparagraph(document.Paragraphs.Last, null);
                }

                foreach (var Pak in order.DishPackagesNoSpis.Where(a => (a.Dish.DishKitсhenGroupId == null)))
                {
                    document.Paragraphs.Add();
                    AddDishparagraph(document.Paragraphs.Last, (DishPackageToGoOrder)Pak);

                }




            }
            catch (Exception e)
            {
                logger.Error($"Error PrintKitchenDocumentToGo {order.Id} Mess: {e.Message}");
            }
        }

        public void PrintKitchenDocumentToFly(OrderFlight order)
        {
            if (order == null) return;
            try
            {
                logger.Debug($"PrintKitchenDocumentToFly {order.Id}");
                // Создаём объект приложения
                Word.Application app = new Word.Application();
                app.Visible = true;
                // Путь до шаблона документа
                // Открываем

                document = app.Documents.Add(System.AppDomain.CurrentDomain.BaseDirectory + SampleDocumentPath);
                document.Paragraphs.Add();
                int ParNum = 1;
                AddLeftCaptionText(document.Paragraphs[1], $"Компания {order.AirCompany.Name}", true);
                document.Paragraphs.Add();

                AddLeftCaptionText(document.Paragraphs.Last, $"Борт {order.FlightNumber}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Дата {order.DeliveryDate.ToString("dd.MM.yyyy HH:mm")}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Доставка: {order.DeliveryPlace?.Name}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Контакты: {order.ContactPerson?.FullSearchData}");
                document.Paragraphs.Add();
                AddLeftCaptionText(document.Paragraphs.Last, $"Машина забирает: {order.ExportTime.ToString("HH:mm")}");
                document.Paragraphs.Add();
                AddMiddleCaptionText(document.Paragraphs.Last, $"Готовность в ресторане в {order.ReadyTime.ToString("HH:mm")}", true);
                string comment = Utils.TextHelper.GetTextFromRadDocText(order.Comment);
                if (comment != null && comment != "")
                {
                    document.Paragraphs.Add();
                    AddMiddleCaptionText(document.Paragraphs.Last, comment);
                }


                foreach (var fn in order.DishPackagesNoSpis.Select(a => ((DishPackageFlightOrder)a).PassageNumber).Distinct().OrderBy(a => a))
                {
                    if (order.DishPackagesNoSpis.Select(a => ((DishPackageFlightOrder)a).PassageNumber).Distinct().Count() > 1)
                    {
                        document.Paragraphs.Add();
                        AddFlightNumberStr(document.Paragraphs.Last, fn);
                    }

                    foreach (var KGroup in order.DishPackagesNoSpis.Where(a => ((DishPackageFlightOrder)a).PassageNumber == fn && a.Dish.DishKitсhenGroupId != null).Select(a => a.Dish.DishKitсhenGroupId).Distinct().OrderBy(a => a.Value))
                    {

                        foreach (var Pak in order.DishPackagesNoSpis.Where(a => ((DishPackageFlightOrder)a).PassageNumber == fn && a.Dish.DishKitсhenGroupId != null && a.Dish.DishKitсhenGroupId == KGroup).OrderBy(a => a.PositionInOrder))
                        {
                            document.Paragraphs.Add();
                            AddDishparagraph(document.Paragraphs.Last, (DishPackageFlightOrder)Pak);
                        }
                        document.Paragraphs.Add();
                        AddDishparagraph(document.Paragraphs.Last, null);
                    }

                    foreach (var Pak in order.DishPackagesNoSpis.Where(a => ((DishPackageFlightOrder)a).PassageNumber == fn && a.Dish.DishKitсhenGroupId == null))
                    {
                        document.Paragraphs.Add();
                        AddDishparagraph(document.Paragraphs.Last, (DishPackageFlightOrder)Pak);

                    }

                }


            }
            catch (Exception e)
            {
                logger.Error($"Error PrintKitchenDocumentToFly {order.Id} Mess: {e.Message}");
            }
        }


        private void AddDishparagraph(Word.Paragraph p, AlohaService.Interfaces.IDishPackageLabel pak)
        {
            string s = "";
            if (pak != null)
            {
                s = $"({pak.Dish.Barcode}) {pak.Dish.Name} - {pak.Amount.ToString("0.##")} порц.";
                try
                {
                    string Comment = "";
                    if (pak is DishPackageFlightOrder dishPackageFlightOrder)
                    {
                        Comment = Utils.TextHelper.GetTextFromRadDocText(pak.Comment);
                    }
                    else if (pak is DishPackageToGoOrder dishPackageToGoOrder)
                    {
                        Comment = pak.Comment;
                    }
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
            p.Range.Text = s;
            p.Range.Font.Size = 12;
            p.Range.Font.Italic = 0;
            p.Range.Font.Bold = 0;
            p.Range.Font.Name = "Times New Roman";
            p.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            p.SpaceAfter = 0;


        }

        /*
        private void AddDishparagraph(Word.Paragraph p, DishPackageToGoOrder pak)
        {
            string s = "";
            if (pak != null)
            {
                s = $"({pak.Dish.Barcode}) {pak.Dish.Name} - {pak.Amount.ToString("0.##")} порц.";
                try
                {
                    string Comment = pak.Comment;
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
            p.Range.Text = s;
            p.Range.Font.Size = 12;
            p.Range.Font.Italic = 0;
            p.Range.Font.Bold = 0;
            p.Range.Font.Name = "Times New Roman";
            p.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            p.SpaceAfter = 0;


        }
        */
        private void AddFlightNumberStr(Word.Paragraph p, int fn)
        {
            p.Range.Text = $"{fn}-й Перелет. Упаковать отдельно, подписать!!!";
            p.Range.Font.Size = 20;
            p.Range.Font.Italic = 0;
            p.Range.Font.Name = "Times New Roman";
            p.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            p.SpaceAfter = 8;
            p.Range.Font.Bold = 1;

        }

        private void AddMiddleCaptionText(Word.Paragraph p, string text, bool bold = false)
        {
            p.Range.Text = text;
            p.Range.Font.Size = 18;
            p.Range.Font.Italic = 1;
            p.Range.Font.Name = "Times New Roman";
            p.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            p.SpaceAfter = 8;
            if (bold)
            { p.Range.Font.Bold = 1; }
            else
            {
                p.Range.Font.Bold = 0;
            }
        }

        private void AddLeftCaptionText(Word.Paragraph p, string text, bool bold = false)
        {
            p.Range.Text = text;
            p.Range.Font.Size = 14;
            p.Range.Font.Italic = 1;
            p.Range.Font.Name = "Times New Roman";
            p.SpaceAfter = 0;
            if (bold)
            { p.Range.Font.Bold = 1; }
            else
            {
                p.Range.Font.Bold = 0;
            }

        }


    }
}
