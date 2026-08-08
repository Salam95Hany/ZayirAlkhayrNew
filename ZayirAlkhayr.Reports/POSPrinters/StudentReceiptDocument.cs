//using System.Globalization;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

//namespace ZayirAlkhayr.Reports.POSPrinters
//{
//    public class StudentReceiptDocument: IDocument
//    {
//        private const string CairoFont = "Cairo";

//        private const float PageWidthMm = 80f;
//        private const float HorizontalMarginMm = 4f;
//        private const float TopMarginMm = 3f;
//        private const float BottomMarginMm = 4f;

//        private readonly StudentReceiptModel _model;
//        private readonly ReceiptBrandingOptions _branding;

//        public StudentReceiptDocument(StudentReceiptModel model,ReceiptBrandingOptions branding)
//        {
//            _model = model ?? throw new ArgumentNullException(nameof(model));
//            _branding = branding ?? throw new ArgumentNullException(nameof(branding));
//        }

//        public DocumentMetadata GetMetadata()
//        {
//            return new DocumentMetadata
//            {
//                Title = $"Student Receipt {_model.Receipt.ReceiptNumber}",
//                Author = _branding.EnglishSchoolName,
//                Subject = "Student payment receipt",
//                Creator = "School Management System"
//            };
//        }

//        public DocumentSettings GetSettings()
//        {
//            return DocumentSettings.Default;
//        }

//        public void Compose(IDocumentContainer container)
//        {
//            container.Page(page =>
//            {
//                // Fixed paper width, dynamically calculated height.
//                page.ContinuousSize(PageWidthMm, Unit.Millimeter);

//                page.MarginHorizontal(HorizontalMarginMm, Unit.Millimeter);
//                page.MarginTop(TopMarginMm, Unit.Millimeter);
//                page.MarginBottom(BottomMarginMm, Unit.Millimeter);

//                page.PageColor(Colors.White);

//                page.ContentFromRightToLeft();

//                page.DefaultTextStyle(style => style
//                    .FontFamily(CairoFont)
//                    .FontSize(8.5f)
//                    .FontColor(Colors.Black)
//                    .LineHeight(1.25f));

//                page.Content()
//                    .Column(column =>
//                    {
//                        column.Spacing(3.5f);

//                        column.Item().Element(ComposeHeader);

//                        column.Item()
//                            .LineHorizontal(0.9f)
//                            .LineColor(Colors.Black);

//                        column.Item().Element(ComposeReceiptTitle);

//                        column.Item().Element(ComposeReceiptMetadata);

//                        column.Item()
//                            .PaddingVertical(1)
//                            .LineHorizontal(0.6f)
//                            .LineDashPattern([3f, 2f])
//                            .LineColor(Colors.Black);

//                        column.Item().Element(ComposeStudentAndGuardian);

//                        column.Item().Element(ComposeFeesTable);

//                        column.Item().Element(ComposeTotals);

//                        column.Item().Element(ComposeAmountInWords);

//                        column.Item()
//                            .PaddingTop(1)
//                            .LineHorizontal(0.6f)
//                            .LineDashPattern([3f, 2f])
//                            .LineColor(Colors.Black);

//                        column.Item().Element(ComposeFooter);
//                    });
//            });
//        }

//        private void ComposeHeader(IContainer container)
//        {
//            container.Column(column =>
//            {
//                column.Spacing(2.5f);

//                column.Item().Row(row =>
//                {
//                    row.Spacing(5);

//                    // RTL: first item appears on the right.
//                    row.RelativeItem(4)
//                        .Column(text =>
//                        {
//                            text.Spacing(0.5f);

//                            text.Item()
//                                .AlignCenter()
//                                .Text(_branding.ArabicSchoolName)
//                                .FontSize(14)
//                                .Bold();

//                            text.Item()
//                                .ContentFromLeftToRight()
//                                .AlignCenter()
//                                .Text(_branding.EnglishSchoolName)
//                                .FontSize(7.8f)
//                                .SemiBold()
//                                .LetterSpacing(0.08f);

//                            text.Item()
//                                .PaddingTop(1)
//                                .AlignCenter()
//                                .Text(_branding.Tagline)
//                                .FontSize(7.8f);
//                        });

//                    row.ConstantItem(48)
//                        .Height(48)
//                        .AlignCenter()
//                        .AlignMiddle()
//                        .Element(ComposeLogo);
//                });

//                column.Item()
//                    .PaddingTop(1.5f)
//                    .Row(row =>
//                    {
//                        row.Spacing(2);

//                        row.RelativeItem()
//                            .Element(x => ComposeHeaderContact(
//                                x,
//                                ReceiptIcons.Phone,
//                                _branding.Phone,
//                                ltr: true));

//                        row.RelativeItem()
//                            .Element(x => ComposeHeaderContact(
//                                x,
//                                ReceiptIcons.Location,
//                                _branding.Location));

//                        row.RelativeItem()
//                            .Element(x => ComposeHeaderContact(
//                                x,
//                                ReceiptIcons.Globe,
//                                _branding.Website,
//                                ltr: true));
//                    });
//            });
//        }

//        private void ComposeLogo(IContainer container)
//        {
//            if (!string.IsNullOrWhiteSpace(_branding.LogoPath) &&
//                File.Exists(_branding.LogoPath))
//            {
//                container
//                    .Svg(_branding.LogoPath)
//                    .FitArea();

//                return;
//            }

//            // Safe fallback if logo was not deployed.
//            container
//                .Border(0.8f)
//                .CornerRadius(4)
//                .AlignCenter()
//                .AlignMiddle()
//                .Text("SCHOOL")
//                .FontSize(7)
//                .Bold();
//        }

//        private static void ComposeHeaderContact(
//            IContainer container,
//            string svg,
//            string value,
//            bool ltr = false)
//        {
//            container
//                .MinHeight(15)
//                .Row(row =>
//                {
//                    row.Spacing(2);

//                    row.ConstantItem(9)
//                        .Height(9)
//                        .AlignMiddle()
//                        .Svg(svg)
//                        .FitArea();

//                    var textContainer = row.RelativeItem()
//                        .AlignMiddle();

//                    if (ltr)
//                        textContainer = textContainer.ContentFromLeftToRight();

//                    textContainer
//                        .Text(value)
//                        .FontSize(6.4f);
//                });
//        }

//        private static void ComposeReceiptTitle(IContainer container)
//        {
//            container
//                .PaddingVertical(1)
//                .AlignCenter()
//                .Text("إيصال سداد")
//                .FontSize(14)
//                .Bold();
//        }

//        private void ComposeReceiptMetadata(IContainer container)
//        {
//            container.Row(row =>
//            {
//                row.Spacing(5);

//                row.RelativeItem()
//                    .Element(ComposeReceiptInfoBlock);

//                row.ConstantItem(0.6f)
//                    .Background(Colors.Grey.Darken1);

//                row.RelativeItem()
//                    .Element(ComposePaymentInfoBlock);
//            });
//        }

//        private void ComposeReceiptInfoBlock(IContainer container)
//        {
//            container.Column(column =>
//            {
//                column.Spacing(2.5f);

//                ComposeKeyValue(
//                    column,
//                    "رقم الإيصال:",
//                    _model.Receipt.ReceiptNumber,
//                    valueLtr: true);

//                ComposeKeyValue(
//                    column,
//                    "تاريخ الإيصال:",
//                    _model.Receipt.ReceiptDate,
//                    valueLtr: true);

//                ComposeKeyValue(
//                    column,
//                    "وقت الإيصال:",
//                    _model.Receipt.ReceiptTime,
//                    valueLtr: true);
//            });
//        }

//        private void ComposePaymentInfoBlock(IContainer container)
//        {
//            container.Column(column =>
//            {
//                column.Spacing(2.5f);

//                ComposeKeyValue(
//                    column,
//                    "نوع السداد:",
//                    _model.Receipt.PaymentType,
//                    badge: true);

//                ComposeKeyValue(
//                    column,
//                    "طريقة السداد:",
//                    _model.Receipt.PaymentMethod);

//                ComposeKeyValue(
//                    column,
//                    "حالة السداد:",
//                    _model.Receipt.PaymentStatus,
//                    badge: true);
//            });
//        }

//        private static void ComposeKeyValue(
//            ColumnDescriptor column,
//            string label,
//            string value,
//            bool valueLtr = false,
//            bool badge = false)
//        {
//            column.Item().Row(row =>
//            {
//                row.Spacing(2);

//                row.RelativeItem(1.1f)
//                    .AlignRight()
//                    .Text(label)
//                    .FontSize(7.3f);

//                var valueContainer = row.RelativeItem(1.35f);

//                if (valueLtr)
//                    valueContainer = valueContainer.ContentFromLeftToRight();

//                if (badge)
//                {
//                    valueContainer
//                        .AlignCenter()
//                        .Background(Colors.Black)
//                        .CornerRadius(3)
//                        .PaddingHorizontal(4)
//                        .PaddingVertical(2)
//                        .Text(value)
//                        .FontSize(7.2f)
//                        .FontColor(Colors.White)
//                        .SemiBold();
//                }
//                else
//                {
//                    valueContainer
//                        .AlignRight()
//                        .Text(value)
//                        .FontSize(7.5f)
//                        .SemiBold();
//                }
//            });
//        }

//        private void ComposeStudentAndGuardian(IContainer container)
//        {
//            container.Row(row =>
//            {
//                row.Spacing(4);

//                row.RelativeItem(1.1f)
//                    .Element(ComposeStudentInfo);

//                row.ConstantItem(0.6f)
//                    .Background(Colors.Grey.Darken1);

//                row.RelativeItem()
//                    .Element(ComposeGuardianInfo);
//            });
//        }

//        private void ComposeStudentInfo(IContainer container)
//        {
//            container.Column(column =>
//            {
//                column.Spacing(2.2f);

//                column.Item()
//                    .Element(x => ComposeSectionHeading(
//                        x,
//                        ReceiptIcons.User,
//                        "بيانات الطالب"));

//                ComposeSimpleField(column, "اسم الطالب:", _model.Student.Name);

//                ComposeSimpleField(
//                    column,
//                    "المرحلة:",
//                    _model.Student.AcademicStage);

//                if (!string.IsNullOrWhiteSpace(_model.Student.Grade))
//                {
//                    ComposeSimpleField(
//                        column,
//                        "الصف:",
//                        _model.Student.Grade!);
//                }

//                ComposeSimpleField(
//                    column,
//                    "رقم الطالب:",
//                    _model.Student.StudentCode,
//                    ltr: true);
//            });
//        }

//        private void ComposeGuardianInfo(IContainer container)
//        {
//            container.Column(column =>
//            {
//                column.Spacing(2.2f);

//                column.Item()
//                    .Element(x => ComposeSectionHeading(
//                        x,
//                        ReceiptIcons.Users,
//                        "بيانات ولي الأمر"));

//                ComposeSimpleField(
//                    column,
//                    "اسم ولي الأمر:",
//                    _model.Guardian.Name);

//                ComposeSimpleField(
//                    column,
//                    "رقم الهاتف:",
//                    _model.Guardian.Phone,
//                    ltr: true);
//            });
//        }

//        private static void ComposeSectionHeading(
//            IContainer container,
//            string icon,
//            string title)
//        {
//            container
//                .PaddingBottom(1)
//                .Row(row =>
//                {
//                    row.Spacing(3);

//                    row.ConstantItem(13)
//                        .Height(13)
//                        .Svg(icon)
//                        .FitArea();

//                    row.RelativeItem()
//                        .AlignMiddle()
//                        .Text(title)
//                        .FontSize(9.5f)
//                        .Bold();
//                });
//        }

//        private static void ComposeSimpleField(
//            ColumnDescriptor column,
//            string label,
//            string value,
//            bool ltr = false)
//        {
//            column.Item().Row(row =>
//            {
//                row.Spacing(2);

//                row.RelativeItem(1)
//                    .Text(label)
//                    .FontSize(7.1f)
//                    .SemiBold();

//                var valueContainer = row.RelativeItem(1.45f);

//                if (ltr)
//                    valueContainer = valueContainer.ContentFromLeftToRight();

//                valueContainer
//                    .Text(value)
//                    .FontSize(7.4f);
//            });
//        }

//        private void ComposeFeesTable(IContainer container)
//        {
//            container
//                .PaddingTop(1)
//                .Table(table =>
//                {
//                    table.ColumnsDefinition(columns =>
//                    {
//                        // Because page is RTL, first logical column is rendered
//                        // on the right side.
//                        columns.RelativeColumn(2.2f); // Fee
//                        columns.RelativeColumn(1.0f); // Total
//                        columns.RelativeColumn(1.0f); // Paid
//                        columns.RelativeColumn(1.0f); // Remaining
//                    });

//                    table.Header(header =>
//                    {
//                        HeaderCell(header.Cell(), "البند");
//                        HeaderCell(header.Cell(), "المبلغ الكلي");
//                        HeaderCell(header.Cell(), "المدفوع");
//                        HeaderCell(header.Cell(), "المتبقي");
//                    });

//                    foreach (var payment in _model.Payments)
//                    {
//                        FeeNameCell(table.Cell(), payment.FeeName);

//                        MoneyCell(
//                            table.Cell(),
//                            payment.TotalAmount);

//                        MoneyCell(
//                            table.Cell(),
//                            payment.PaidAmount);

//                        MoneyCell(
//                            table.Cell(),
//                            payment.RemainingAmount);
//                    }
//                });
//        }

//        private static void HeaderCell(IContainer container, string text)
//        {
//            container
//                .Background(Colors.Black)
//                .MinHeight(22)
//                .PaddingHorizontal(2)
//                .PaddingVertical(3)
//                .AlignCenter()
//                .AlignMiddle()
//                .Text(text)
//                .FontSize(7.2f)
//                .SemiBold()
//                .FontColor(Colors.White);
//        }

//        private static void FeeNameCell(
//            IContainer container,
//            string value)
//        {
//            container
//                .BorderBottom(0.45f)
//                .BorderColor(Colors.Grey.Medium)
//                .MinHeight(22)
//                .PaddingHorizontal(2)
//                .PaddingVertical(3)
//                .AlignRight()
//                .AlignMiddle()
//                .Text(value)
//                .FontSize(7.6f);
//        }

//        private static void MoneyCell(
//            IContainer container,
//            decimal value)
//        {
//            container
//                .BorderBottom(0.45f)
//                .BorderColor(Colors.Grey.Medium)
//                .MinHeight(22)
//                .PaddingHorizontal(1)
//                .PaddingVertical(3)
//                .ContentFromLeftToRight()
//                .AlignCenter()
//                .AlignMiddle()
//                .Text(FormatMoney(value))
//                .FontSize(7.6f);
//        }

//        private void ComposeTotals(IContainer container)
//        {
//            container
//                .Border(0.9f)
//                .BorderColor(Colors.Black)
//                .CornerRadius(4)
//                .Row(row =>
//                {
//                    row.RelativeItem()
//                        .Element(x => ComposeTotalBox(
//                            x,
//                            "المتبقي",
//                            _model.TotalRemaining));

//                    row.ConstantItem(0.6f)
//                        .Background(Colors.Grey.Medium);

//                    row.RelativeItem()
//                        .Element(x => ComposeTotalBox(
//                            x,
//                            "إجمالي المدفوع",
//                            _model.TotalPaid));

//                    row.ConstantItem(0.6f)
//                        .Background(Colors.Grey.Medium);

//                    row.RelativeItem()
//                        .Element(x => ComposeTotalBox(
//                            x,
//                            "إجمالي المبلغ",
//                            _model.TotalAmount));
//                });
//        }

//        private static void ComposeTotalBox(
//            IContainer container,
//            string title,
//            decimal value)
//        {
//            container
//                .PaddingVertical(4)
//                .PaddingHorizontal(2)
//                .Column(column =>
//                {
//                    column.Spacing(1);

//                    column.Item()
//                        .AlignCenter()
//                        .Text(title)
//                        .FontSize(7.2f)
//                        .Bold();

//                    column.Item()
//                        .ContentFromLeftToRight()
//                        .AlignCenter()
//                        .Text(FormatMoney(value))
//                        .FontSize(10)
//                        .Bold();
//                });
//        }

//        private void ComposeAmountInWords(IContainer container)
//        {
//            container
//                .PaddingVertical(1)
//                .Column(column =>
//                {
//                    column.Spacing(2);

//                    column.Item()
//                        .Text("المبلغ المدفوع كتابة:")
//                        .FontSize(7.6f)
//                        .SemiBold();

//                    column.Item()
//                        .PaddingHorizontal(2)
//                        .Text(_model.TotalPaidText)
//                        .FontSize(8.2f)
//                        .SemiBold();
//                });
//        }

//        private static void ComposeFooter(IContainer container)
//        {
//            container
//                .PaddingTop(2)
//                .Column(column =>
//                {
//                    column.Spacing(2);

//                    column.Item()
//                        .AlignCenter()
//                        .Text("شكراً لثقتكم بنا")
//                        .FontSize(8.5f)
//                        .Bold();

//                    column.Item()
//                        .AlignCenter()
//                        .Text("يرجى الاحتفاظ بالإيصال للمراجعة عند الحاجة.")
//                        .FontSize(7.2f);

//                    // Feed area after the receipt body.
//                    column.Item().Height(8);
//                });
//        }

//        private static string FormatMoney(decimal amount)
//        {
//            return amount.ToString(
//                "#,##0.00",
//                CultureInfo.InvariantCulture);
//        }
//    }
//}
