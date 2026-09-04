using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZayirAlkhayr.Entities.POSPrinters;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Reports.Service
{
    public sealed class StudentProfilePdf: IDocument
    {
        private const string Navy = "#073B78";
        private const string NavyDark = "#062F62";
        private const string Border = "#D8E1EC";
        private const string LightBlue = "#F4F8FD";
        private const string Text = "#14213D";
        private const string Muted = "#64748B";
        private const string Green = "#169B62";
        private const string GreenBg = "#EAF8F0";
        private const string Orange = "#E69200";
        private const string OrangeBg = "#FFF8E8";
        private const string Red = "#E43D3D";
        private const string RedBg = "#FFF0F0";
        private const string BlueBg = "#EEF5FF";
        private const string PurpleBg = "#F3EFFF";

        private readonly StudentProfilePdfModel _model;
        private readonly ReceiptBrandingOptions _branding;

        public StudentProfilePdf(StudentProfilePdfModel model, ReceiptBrandingOptions branding)
        {
            _model = model;
            _branding = branding;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            ComposePage(container, 1, ComposeFirstPage);
            ComposePage(container, 2, ComposeSecondPage);
        }

        private void ComposePage(IDocumentContainer container, int pageNumber, Action<IContainer> content)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(10);
                page.PageColor(Colors.White);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(9).FontColor(Text));

                page.Header().Height(82).Element(ComposeHeader);
                page.Content().PaddingVertical(8).Element(content);
                page.Footer().Height(30).Element(x => ComposeFooter(x, pageNumber));
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container
                .Border(1)
                .BorderColor(Navy)
                .PaddingHorizontal(14)
                .PaddingVertical(8)
                .Row(row =>
                {
                    row.RelativeItem(3).AlignMiddle().Row(brand =>
                    {
                        brand.AutoItem().AlignMiddle().Width(54).Height(54).Element(ComposeLogo);
                        brand.RelativeItem().PaddingRight(8).AlignMiddle().Column(col =>
                        {
                            col.Item().Text(_branding.ArabicSchoolName).FontSize(13).SemiBold().FontColor(NavyDark);
                            col.Item().PaddingTop(3).Text(_branding.Tagline).FontSize(8).FontColor("#4E79A7");
                        });
                    });

                    row.RelativeItem(4).AlignCenter().AlignMiddle().Column(col =>
                    {
                        col.Item().AlignCenter().Height(16).Svg(MortarboardSvg).FitArea();
                        col.Item().PaddingTop(1).AlignCenter().Text("ملف الطالب").FontSize(15).Bold().FontColor(NavyDark);
                        col.Item().PaddingTop(1).AlignCenter().Text(_model.AcademicYear).FontSize(8).FontColor(Navy);
                    });

                    row.RelativeItem(3).AlignMiddle().AlignLeft().Column(col =>
                    {
                        col.Item().AlignLeft().Text("كود الطالب").FontSize(8).FontColor(Muted);
                        col.Item().PaddingTop(2).AlignLeft().ContentFromLeftToRight().Text(_model.AcademicNumber).FontSize(11).Bold().FontColor(NavyDark);
                    });
                });
        }

        private void ComposeLogo(IContainer container)
        {
            if (!string.IsNullOrWhiteSpace(_branding.SchoolLogoPath) && File.Exists(_branding.SchoolLogoPath))
                container.Image(_branding.SchoolLogoPath).FitArea();
            else
                container.Border(1).BorderColor(Navy).CornerRadius(8).AlignCenter().AlignMiddle()
                    .Text("LOGO").Bold().FontSize(9).FontColor(Navy);
        }

        private void ComposeFirstPage(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(8);

                column.Item().Row(row =>
                {
                    row.RelativeItem(5.8f).Element(ComposeStudentBasicData);
                    row.ConstantItem(10);
                    row.RelativeItem(2.2f).Element(ComposeStudentQuickInfo);
                });

                column.Item().Element(c => ComposeSection(c, "البيانات الأكاديمية", ComposeAcademicData));

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeSection(c, "ولي الأمر", ComposeGuardian));
                    row.ConstantItem(10);
                    row.RelativeItem().Element(c => ComposeSection(c, "معلومات الاتصال و محل الإقامة", ComposeContact));
                });
            });
        }

        private void ComposeStudentBasicData(IContainer container)
        {
            ComposeSection(container, "بيانات الطالب الأساسية", body =>
            {
                var items = new (string Label, string Value)[]
                {
                    ("الاسم الكامل", _model.FullName),
                    ("تاريخ الميلاد", _model.BirthDate.ToString("yyyy/MM/dd")),
                    ("العمر", _model.Age.ToString()),
                    ("الجنسية", _model.Nationality),
                    ("الحالة الصحية", _model.HealthStatus),
                    ("عدد الإخوة", _model.SiblingsCount.ToString())
                };

                body.Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(92);
                        cols.RelativeColumn();
                    });

                    foreach (var item in items)
                    {
                        table.Cell().Element(LabelCell).Text(item.Label).SemiBold();
                        table.Cell().Element(ValueCell).Text(item.Value);
                    }
                });
            });
        }

        private void ComposeStudentQuickInfo(IContainer container)
        {
            container.PaddingTop(14).Column(col =>
            {
                col.Spacing(8);
                col.Item().Text(_model.FullName).FontSize(15).Bold().FontColor("#111827");
                col.Item().Element(Divider);
                col.Item().Element(c => QuickInfo(c, "الفترة الدراسية", _model.Grade));
                col.Item().Element(c => QuickInfo(c, "الحالة", _model.Status, true));
            });
        }

        private void ComposeAcademicData(IContainer container)
        {
            container.PaddingVertical(4).Row(row =>
            {
                AcademicItem(row, "العام الدراسي", _model.AcademicYear);
                AcademicItem(row, "المرحلة", _model.Stage);
                AcademicItem(row, "الفترة الدراسية", _model.Grade);
                AcademicItem(row, "تاريخ التسجيل", _model.RegistrationDate.ToString("yyyy/MM/dd"));
            });
        }

        private void ComposeGuardian(IContainer container)
        {
            KeyValueTable(container, new[]
            {
                ("الاسم", _model.ParentName),
                ("صلة القرابة", _model.Relationship),
                ("رقم الهاتف", _model.ParentPhone)
            });
        }

        private void ComposeContact(IContainer container)
        {
            KeyValueTable(container, new[]
            {
                ("العنوان الكامل", _model.Adress),
                ("رقم الهاتف", _model.ParentPhone),
                ("رقم الواتساب", _model.ParentWhatsappNumber)
            });
        }

        private void ComposeSecondPage(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(9);
                column.Item().Element(ComposeFeeSummary);
                column.Item().Row(row =>
                {
                    row.RelativeItem(1.25f).Element(c => ComposeSection(c, "تفاصيل الرسوم", ComposeFeesTable));
                    //row.ConstantItem(10);
                    //row.RelativeItem(1f).Element(c => ComposeSection(c, "آخر الأنشطة", ComposeActivities));
                });

                column.Item().Element(c => ComposeSection(c, "ملاحظات عامة", ComposeGeneralNotes));
            });
        }

        private void ComposeFeeSummary(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().AlignRight().Text("ملخص الرسوم").FontSize(10).SemiBold().FontColor(NavyDark);
                col.Item().PaddingTop(5).Row(row =>
                {
                    SummaryCard(row, "إجمالي الرسوم", Money(_model.Fees.Total), Navy, BlueBg);
                    SummaryCard(row, "إجمالي المدفوع", Money(_model.Fees.Paid), Green, GreenBg);
                    SummaryCard(row, "المتبقي", Money(_model.Fees.Remaining), Orange, OrangeBg);
                    SummaryCard(row, "حالة السداد", _model.Fees.PaymentStatus, Red, RedBg);  
                });
            });
        }

        private void ComposeFeesTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(28);
                    cols.RelativeColumn(1.7f);
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.ConstantColumn(70);
                });

                foreach (var h in new[] { "#", "نوع الرسوم", "القيمة", "تاريخ الاستحقاق", "المبلغ المدفوع", "المتبقي", "الحالة" })
                    table.Cell().Element(TableHeaderCell).Text(h).SemiBold();

                foreach (var item in _model.Fees.Items)
                {
                    table.Cell().Element(TableBodyCell).Text(item.Number.ToString());
                    table.Cell().Element(TableBodyCell).Text(item.FeeType);
                    table.Cell().Element(TableBodyCell).Text(Money(item.Amount));
                    table.Cell().Element(TableBodyCell).Text(item.DueDate);
                    table.Cell().Element(TableBodyCell).Text(Money(item.PaidAmount));
                    table.Cell().Element(TableBodyCell).Text(Money(item.RemainingAmount));
                    table.Cell().Element(TableBodyCell).AlignCenter().Element(c => StatusBadge(c, item.Status));
                }
            });
        }

        //private void ComposeActivities(IContainer container)
        //{
        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(cols =>
        //        {
        //            cols.ConstantColumn(74);
        //            cols.RelativeColumn();
        //            cols.ConstantColumn(54);
        //        });

        //        //foreach (var h in new[] { "التاريخ", "النشاط", "النوع" })
        //        //    table.Cell().Element(TableHeaderCell).Text(h).SemiBold();

        //        //foreach (var item in _model.Activities)
        //        //{
        //        //    table.Cell().Element(TableBodyCell).Text(item.Date.ToString("yyyy/MM/dd"));
        //        //    table.Cell().Element(TableBodyCell).Text(item.Description);
        //        //    table.Cell().Element(TableBodyCell).AlignCenter().Element(c => ActivityBadge(c, item.Type));
        //        //}
        //    });
        //}

        private void ComposeGeneralNotes(IContainer container)
        {
            container.Padding(8).Column(col =>
            {
                col.Spacing(5);
                foreach (var note in _model.GeneralNotes)
                {
                    col.Item().Row(row =>
                    {
                        row.AutoItem().PaddingTop(2).Text("•").FontSize(12).FontColor(Navy);
                        row.RelativeItem().PaddingRight(5).Text(note).FontSize(9.5f);
                    });
                }
            });
        }

        private void ComposeFooter(IContainer container, int pageNumber)
        {
            container.Background(NavyDark).PaddingHorizontal(14).Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text($"تاريخ إصدار التقرير: {_model.ReportDate:yyyy/MM/dd}").FontSize(7.5f).FontColor(Colors.White);
                row.ConstantItem(80).AlignCenter().AlignMiddle().Background(Colors.White).CornerRadius(8).ContentFromLeftToRight().Text($"{pageNumber} / 2")
                    .FontSize(10).Bold().FontColor(NavyDark);
                row.RelativeItem().AlignLeft().AlignMiddle().Row(phoneRow =>
                {
                    phoneRow.AutoItem().ContentFromRightToLeft().Text($"{_branding.Location}    |").FontSize(7.2f).FontColor(Colors.White);
                    phoneRow.AutoItem().ContentFromLeftToRight().Text(_branding.Phone).FontSize(7.2f).FontColor(Colors.White);
                });
            });
        }

        private static void ComposeSection(IContainer container, string title, Action<IContainer> body)
        {
            container.Border(1).BorderColor(Border).CornerRadius(7).Column(col =>
            {
                col.Item().AlignRight().Element(c =>
                {
                    c.AlignRight().Background(NavyDark).CornerRadius(5).PaddingHorizontal(16).PaddingVertical(5)
                        .Text(title).FontSize(10.5f).SemiBold().FontColor(Colors.White);
                });

                col.Item().Padding(7).Element(body);
            });
        }

        private static void KeyValueTable(IContainer container, IEnumerable<(string Label, string Value)> rows)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(95);
                    cols.RelativeColumn();
                });

                foreach (var row in rows)
                {
                    table.Cell().Element(LabelCell).Text(row.Label).SemiBold();
                    table.Cell().Element(ValueCell).Text(row.Value);
                }
            });
        }

        private static void QuickInfo(IContainer container, string label, string value, bool badge = false)
        {
            container.Column(col =>
            {
                col.Item().Text(label).FontSize(7.5f).FontColor(Muted);
                if (badge)
                {
                    col.Item().PaddingTop(2).AlignRight().Background(GreenBg).CornerRadius(8).PaddingHorizontal(10).PaddingVertical(3)
                        .Text(value).FontSize(8.5f).SemiBold().FontColor(Green);
                }
                else
                {
                    col.Item().PaddingTop(2).Text(value).FontSize(9).SemiBold();
                }
            });
        }

        private static void AcademicItem(RowDescriptor row, string label, string value)
        {
            row.RelativeItem().BorderLeft(1).BorderColor(Border).PaddingHorizontal(8).AlignCenter().Column(col =>
            {
                col.Item().AlignCenter().Text(label).FontSize(7.5f).SemiBold().FontColor(Navy);
                col.Item().PaddingTop(4).AlignCenter().Text(value).FontSize(8.5f);
            });
        }

        private static void SummaryCard(RowDescriptor row, string label, string value, string accent, string background)
        {
            row.RelativeItem().PaddingHorizontal(4).Border(1).BorderColor(accent).CornerRadius(6).Background(background)
                .PaddingVertical(10).PaddingHorizontal(12).Column(col =>
                {
                    col.Item().AlignCenter().Text(label).FontSize(9).SemiBold().FontColor(accent);
                    col.Item().PaddingTop(5).AlignCenter().Text(value).FontSize(12).Bold().FontColor(NavyDark);
                });
        }

        private static void StatusBadge(IContainer container, string status)
        {
            var isPaid = status.Contains("مدفوع", StringComparison.OrdinalIgnoreCase);
            container.Background(isPaid ? GreenBg : RedBg).CornerRadius(7).PaddingHorizontal(7).PaddingVertical(2)
                .Text(status).FontSize(7.5f).SemiBold().FontColor(isPaid ? Green : Red);
        }

        private static IContainer LabelCell(IContainer container) =>
            container.BorderBottom(1).BorderColor(Border).Background(LightBlue).PaddingHorizontal(7).PaddingVertical(5).AlignMiddle();

        private static IContainer ValueCell(IContainer container) =>
            container.BorderBottom(1).BorderColor(Border).PaddingHorizontal(7).PaddingVertical(5).AlignMiddle();

        private static IContainer TableHeaderCell(IContainer container) =>
            container.Border(1).BorderColor(Border).Background(LightBlue).PaddingHorizontal(4).PaddingVertical(5).AlignCenter().AlignMiddle();

        private static IContainer TableBodyCell(IContainer container) =>
            container.Border(1).BorderColor(Border).PaddingHorizontal(4).PaddingVertical(5).AlignCenter().AlignMiddle();

        private static void Divider(IContainer container) => container.Height(1).Background(Border);

        private static string Money(decimal value) => $"{value:N2} ج.م";

        private const string MortarboardSvg = """
        <svg viewBox="0 0 64 40" xmlns="http://www.w3.org/2000/svg">
          <path fill="#073B78" d="M32 2 3 15l29 13 24-10.8V30h4V15L32 2Z"/>
          <path fill="#073B78" d="M15 22v8c0 5 8 8 17 8s17-3 17-8v-8l-17 8-17-8Z"/>
        </svg>
        """;
    }
}
