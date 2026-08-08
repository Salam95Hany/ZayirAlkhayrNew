using Microsoft.Extensions.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using ZayirAlkhayr.Entities.POSPrinters;

namespace ZayirAlkhayr.Reports.POSPrinters
{
    public class StudentReceiptDocument : IDocument
    {
        private const string CairoFont = "Cairo";

        private const float PageWidthMm = 80f;
        private const float HorizontalMarginMm = 2f;
        private const float TopMarginMm = 3f;
        private const float BottomMarginMm = 4f;

        private readonly StudentReceiptModel _model;
        private readonly ReceiptBrandingOptions _branding;

        public StudentReceiptDocument(StudentReceiptModel model, ReceiptBrandingOptions branding)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _branding = branding ?? throw new ArgumentNullException(nameof(branding));
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = $"Student Receipt {_model.StudentReceipt.ReceiptNumber}",
                Author = _branding.EnglishSchoolName,
                Subject = "Student payment receipt",
                Creator = "School Management System"
            };
        }

        public DocumentSettings GetSettings()
        {
            return DocumentSettings.Default;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.ContinuousSize(PageWidthMm, Unit.Millimetre);
                page.MarginHorizontal(HorizontalMarginMm, Unit.Millimetre);
                page.MarginTop(TopMarginMm, Unit.Millimetre);
                page.MarginBottom(BottomMarginMm, Unit.Millimetre);
                page.PageColor(Colors.White);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(style => style.FontFamily(CairoFont).FontSize(8.5f).FontColor(Colors.Black).LineHeight(1.25f));
                page.Content().Column(column =>
                {
                    column.Spacing(3.5f);
                    column.Item().Element(ComposeHeader);
                    column.Item().LineHorizontal(0.9f).LineColor(Colors.Black);
                    column.Item().Element(ComposeReceiptTitle);
                    column.Item().Element(ComposeReceiptMetadata);
                    column.Item().PaddingVertical(1).LineHorizontal(0.6f).LineDashPattern([3f, 2f]).LineColor(Colors.Black);
                    column.Item().Element(ComposeStudentAndGuardian);
                    column.Item().Element(ComposeFeesTable);
                    column.Item().Element(ComposeTotals);
                    column.Item().Element(ComposeAmountInWords);
                    column.Item().PaddingTop(1).LineHorizontal(0.6f).LineDashPattern([3f, 2f]).LineColor(Colors.Black);
                    column.Item().Element(ComposeFooter);
                });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                //column.Spacing(2.5f);
                column.Item().Row(row =>
                {
                    row.Spacing(5);
                    row.RelativeItem(4).Column(text =>
                    {
                        text.Spacing(0.5f);
                        text.Item().AlignCenter().Text(_branding.ArabicSchoolName).FontSize(14).Bold();
                        text.Item().ContentFromLeftToRight().AlignCenter().Text(_branding.EnglishSchoolName).FontSize(7.8f).SemiBold().LetterSpacing(0.08f);
                        text.Item().PaddingTop(1).AlignCenter().Text(_branding.Tagline).FontSize(7.8f);
                    });
                    row.ConstantItem(48).Height(48).AlignCenter().AlignMiddle().Element(ComposeLogo);
                });
                column.Item().PaddingTop(1.5f).Row(row =>
                {
                    row.Spacing(2);
                    row.RelativeItem().Element(x => ComposeHeaderContact(x,ReceiptIcons.Phone,_branding.Phone,ltr: true));
                    row.RelativeItem().Element(x => ComposeHeaderContact(x,ReceiptIcons.Location,_branding.Location));
                });
            });
        }

        private void ComposeLogo(IContainer container)
        {
            container.Column(column =>
            {
                if (!string.IsNullOrWhiteSpace(_branding.LogoPath) &&File.Exists(_branding.LogoPath))
                {
                    var logoBytes = File.ReadAllBytes(_branding.LogoPath);
                    column.Item().Width(48).AlignCenter().Image(logoBytes);
                }
            });
        }

        private static void ComposeHeaderContact(IContainer container,string svg,string value,bool ltr = false)
        {
            container.MinHeight(15).Row(row =>
            {
                row.Spacing(2);
                row.ConstantItem(9).Height(9).AlignMiddle().Svg(svg).FitArea();
                var textContainer = row.RelativeItem().AlignMiddle().AlignRight();
                if (ltr)
                    textContainer = textContainer.ContentFromLeftToRight();

                textContainer.Text(value).FontSize(6.4f);
            });
        }

        private static void ComposeReceiptTitle(IContainer container)
        {
            container.PaddingVertical(1).AlignCenter().Text("إيصال سداد").FontSize(14).Bold();
        }

        private void ComposeReceiptMetadata(IContainer container)
        {
            container.Row(row =>
            {
                row.Spacing(5);
                row.RelativeItem().Element(ComposeReceiptInfoBlock);
                row.ConstantItem(0.6f).Background(Colors.Grey.Darken1);
                row.RelativeItem().Element(ComposePaymentInfoBlock);
            });
        }

        private void ComposeReceiptInfoBlock(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2.5f);
                ComposeKeyValue( column,"رقم الإيصال:",_model.StudentReceipt.ReceiptNumber,valueLtr: true);
                ComposeKeyValue(column,"تاريخ الإيصال:",_model.StudentReceipt.ReceiptDate,valueLtr: true);
                ComposeKeyValue(column,"وقت الإيصال:",_model.StudentReceipt.ReceiptTime,valueLtr: true);
            });
        }

        private void ComposePaymentInfoBlock(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2.5f);
                ComposeKeyValue(column,"نوع السداد:",_model.StudentReceipt.PaymentType,badge: true);
                ComposeKeyValue(column,"طريقة السداد:",_model.StudentReceipt.PaymentMethod);
                ComposeKeyValue(column,"حالة السداد:",_model.StudentReceipt.PaymentStatus,badge: true);
            });
        }

        private static void ComposeKeyValue(ColumnDescriptor column,string label,string value,bool valueLtr = false,bool badge = false)
        {
            column.Item().Row(row =>
            {
                row.Spacing(2);
                row.RelativeItem(1.1f).AlignRight().Text(label).FontSize(7.3f);

                var valueContainer = row.RelativeItem(1.35f);
                if (valueLtr)
                    valueContainer = valueContainer.ContentFromLeftToRight();

                if (badge)
                {
                    valueContainer.AlignRight().Background(Colors.Black).CornerRadius(3).PaddingHorizontal(4).PaddingVertical(2).Text(value).FontSize(7.2f)
                        .FontColor(Colors.White)
                        .SemiBold();
                }
                else
                    valueContainer.AlignRight().Text(value).FontSize(7.5f).SemiBold();
            });
        }

        private void ComposeStudentAndGuardian(IContainer container)
        {
            container.Row(row =>
            {
                row.Spacing(4);
                row.RelativeItem(1.1f).Element(ComposeStudentInfo);
                row.ConstantItem(0.6f).Background(Colors.Grey.Darken1);
                row.RelativeItem().Element(ComposeGuardianInfo);
            });
        }

        private void ComposeStudentInfo(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2.2f);
                column.Item().Element(x => ComposeSectionHeading(x,ReceiptIcons.User,"بيانات الطالب"));
                ComposeSimpleField(column, "الطالب:", _model.StudentName);
                ComposeSimpleField(column,"المرحلة:",_model.AcademicStage);
                ComposeSimpleField(column,"السنة الدراسية:",_model.AcademicYear);
                ComposeSimpleField(column,"كود الطالب:", _model.StudentCode,ltr: true);
            });
        }

        private void ComposeGuardianInfo(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2.2f);
                column.Item().Element(x => ComposeSectionHeading(x,ReceiptIcons.Users,"بيانات ولي الأمر"));
                ComposeSimpleField(column,"ولي الأمر:", _model.ParentName);
                ComposeSimpleField(column,"رقم الهاتف:",_model.ParentPhone,ltr: true);
            });
        }

        private static void ComposeSectionHeading(IContainer container,string icon,string title)
        {
            container.PaddingBottom(1).Row(row =>
            {
                row.Spacing(3);
                row.ConstantItem(13).Height(13).Svg(icon).FitArea();
                row.RelativeItem().AlignMiddle().Text(title).FontSize(9.5f).Bold();
            });
        }

        private static void ComposeSimpleField(ColumnDescriptor column,string label,string value,bool ltr = false)
        {
            column.Item().Row(row =>
            {
                row.Spacing(2);
                row.AutoItem().Text(label).FontSize(7.1f).SemiBold();

                var valueContainer = row.RelativeItem();
                if (ltr)
                    valueContainer = valueContainer.ContentFromLeftToRight();

                valueContainer.AlignRight().Text(value).FontSize(7.4f);
            });
        }

        private void ComposeFeesTable(IContainer container)
        {
            container.PaddingTop(1).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.8f);
                        columns.RelativeColumn(1.0f);
                        columns.RelativeColumn(1.0f);
                        columns.RelativeColumn(1.0f);
                    });
                    table.Header(header =>
                    {
                        HeaderCell(header.Cell(), "البند");
                        HeaderCell(header.Cell(), "المبلغ الكلي");
                        HeaderCell(header.Cell(), "المدفوع");
                        HeaderCell(header.Cell(), "المتبقي");
                    });

                    foreach (var payment in _model.StudentPayments)
                    {
                        FeeNameCell(table.Cell(), payment.FeeName);
                        MoneyCell(table.Cell(),payment.TotalAmount);
                        MoneyCell(table.Cell(),payment.PaidAmount);
                        MoneyCell(table.Cell(),payment.RemainingAmount);
                    }
                });
        }

        private static void HeaderCell(IContainer container, string text)
        {
            container.Background(Colors.Black).MinHeight(22).PaddingHorizontal(2).PaddingVertical(3).AlignCenter().AlignMiddle().Text(text).FontSize(7.2f).SemiBold()
                .FontColor(Colors.White);
        }

        private static void FeeNameCell(IContainer container,string value)
        {
            container.BorderBottom(0.45f).BorderColor(Colors.Grey.Medium).MinHeight(22).PaddingHorizontal(2).PaddingVertical(3).AlignRight().AlignMiddle().Text(value)
                .FontSize(7.6f);
        }

        private static void MoneyCell(IContainer container,decimal value)
        {
            container.BorderBottom(0.45f).BorderColor(Colors.Grey.Medium).MinHeight(22).PaddingHorizontal(1).PaddingVertical(3).ContentFromLeftToRight().AlignCenter()
                .AlignMiddle()
                .Text(FormatMoney(value))
                .FontSize(7.6f);
        }

        private void ComposeTotals(IContainer container)
        {
            container.Border(0.9f).BorderColor(Colors.Black).CornerRadius(4).Row(row =>
            {
                row.RelativeItem().Element(x => ComposeTotalBox(x, "إجمالي المبلغ", _model.TotalAmount));
                row.ConstantItem(0.6f).Background(Colors.Grey.Medium);
                row.RelativeItem().Element(x => ComposeTotalBox(x, "إجمالي المدفوع", _model.TotalPaid));
                row.ConstantItem(0.6f).Background(Colors.Grey.Medium);
                row.RelativeItem().Element(x => ComposeTotalBox(x,"المتبقي",_model.TotalRemaining)); 
            });
        }

        private static void ComposeTotalBox(IContainer container,string title,decimal value)
        {
            container.PaddingVertical(4).PaddingHorizontal(2).Column(column =>
            {
                column.Spacing(1);
                column.Item().AlignCenter().Text(title).FontSize(7.2f).Bold();
                column.Item().ContentFromLeftToRight().AlignCenter().Text(FormatMoney(value)).FontSize(10).Bold();
            });
        }

        private void ComposeAmountInWords(IContainer container)
        {
            container.PaddingVertical(1).Row(row =>
            {
                row.Spacing(2);
                row.AutoItem().AlignMiddle().Text("المبلغ المدفوع كتابة:").FontSize(7.5f).SemiBold();
                row.RelativeItem().AlignMiddle().Text(_model.TotalPaidTxt).FontSize(6.5f).SemiBold();
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.PaddingTop(2).Column(column =>
            {
                column.Spacing(2);
                column.Item().AlignCenter().Text("شكراً لثقتكم بنا").FontSize(8.5f).Bold();
                column.Item().AlignCenter().Text("يرجى الاحتفاظ بالإيصال للمراجعة عند الحاجة.").FontSize(7.2f);
                column.Item().Height(8);
            });
        }

        private static string FormatMoney(decimal amount)
        {
            return amount.ToString("#,##0.00",CultureInfo.InvariantCulture);
        }
    }
}
