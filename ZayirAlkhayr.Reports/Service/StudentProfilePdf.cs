using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ZayirAlkhayr.Reports.Service
{
    //public class StudentProfilePdf
    //{
    //    private const string Navy = "#073B78";
    //    private const string NavyDark = "#062F62";
    //    private const string Border = "#D8E1EC";
    //    private const string LightBlue = "#F4F8FD";
    //    private const string Text = "#14213D";
    //    private const string Muted = "#64748B";
    //    private const string Green = "#169B62";
    //    private const string GreenBg = "#EAF8F0";
    //    private const string Orange = "#E69200";
    //    private const string OrangeBg = "#FFF8E8";
    //    private const string Red = "#E43D3D";
    //    private const string RedBg = "#FFF0F0";
    //    private const string BlueBg = "#EEF5FF";
    //    private const string PurpleBg = "#F3EFFF";

    //    private readonly StudentProfilePdfModel _model;

    //    public StudentProfilePdf(StudentProfilePdfModel model)
    //    {
    //        _model = model;
    //    }

    //    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    //    public void Compose(IDocumentContainer container)
    //    {
    //        ComposePage(container, 1, ComposeFirstPage);
    //        ComposePage(container, 2, ComposeSecondPage);
    //    }

    //    private void ComposePage(IDocumentContainer container, int pageNumber, Action<IContainer> content)
    //    {
    //        container.Page(page =>
    //        {
    //            page.Size(PageSizes.A4.Landscape());
    //            page.Margin(10);
    //            page.PageColor(Colors.White);
    //            page.ContentFromRightToLeft();
    //            page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(9).FontColor(Text));

    //            page.Header().Height(76).Element(ComposeHeader);
    //            page.Content().PaddingVertical(8).Element(content);
    //            page.Footer().Height(30).Element(x => ComposeFooter(x, pageNumber));
    //        });
    //    }

    //    private void ComposeHeader(IContainer container)
    //    {
    //        container
    //            .Border(1)
    //            .BorderColor(Navy)
    //            .PaddingHorizontal(14)
    //            .PaddingVertical(8)
    //            .Row(row =>
    //            {
    //                row.RelativeItem(3).AlignMiddle().Row(brand =>
    //                {
    //                    brand.AutoItem().AlignMiddle().Width(54).Height(54).Element(ComposeLogo);
    //                    brand.RelativeItem().PaddingRight(8).AlignMiddle().Column(col =>
    //                    {
    //                        col.Item().Text(_model.School.ArabicName).FontSize(13).SemiBold().FontColor(NavyDark);
    //                        col.Item().PaddingTop(3).Text(_model.School.Slogan).FontSize(8).FontColor("#4E79A7");
    //                    });
    //                });

    //                row.RelativeItem(4).AlignCenter().AlignMiddle().Column(col =>
    //                {
    //                    col.Item().AlignCenter().Height(24).Svg(MortarboardSvg).FitArea();
    //                    col.Item().AlignCenter().Text("ملف الطالب").FontSize(20).Bold().FontColor(NavyDark);
    //                    col.Item().AlignCenter().Text(_model.Academic.AcademicYear).FontSize(9).FontColor(Navy);
    //                });

    //                row.RelativeItem(3).AlignMiddle().AlignLeft().Column(col =>
    //                {
    //                    col.Item().AlignLeft().Text("الرقم الأكاديمي").FontSize(8).FontColor(Muted);
    //                    col.Item().PaddingTop(2).AlignLeft().Text(_model.Student.AcademicNumber)
    //                        .FontSize(11).Bold().FontColor(NavyDark).ContentFromLeftToRight();
    //                });
    //            });
    //    }

    //    private void ComposeLogo(IContainer container)
    //    {
    //        if (!string.IsNullOrWhiteSpace(_model.LogoPath) && File.Exists(_model.LogoPath))
    //            container.Image(_model.LogoPath).FitArea();
    //        else
    //            container.Border(1).BorderColor(Navy).CornerRadius(8).AlignCenter().AlignMiddle()
    //                .Text("LOGO").Bold().FontSize(9).FontColor(Navy);
    //    }

    //    private void ComposeFirstPage(IContainer container)
    //    {
    //        container.Column(column =>
    //        {
    //            column.Spacing(8);

    //            column.Item().Row(row =>
    //            {
    //                row.RelativeItem(5.8f).Element(ComposeStudentBasicData);
    //                row.ConstantItem(10);
    //                row.RelativeItem(2.2f).Element(ComposeStudentQuickInfo);
    //                row.ConstantItem(10);
    //                row.ConstantItem(128).Element(ComposeStudentPhoto);
    //            });

    //            column.Item().Element(c => ComposeSection(c, "البيانات الأكاديمية", ComposeAcademicData));

    //            column.Item().Row(row =>
    //            {
    //                row.RelativeItem().Element(c => ComposeSection(c, "ولي الأمر", ComposeGuardian));
    //                row.ConstantItem(10);
    //                row.RelativeItem().Element(c => ComposeSection(c, "معلومات الاتصال و محل الإقامة", ComposeContact));
    //            });
    //        });
    //    }

    //    private void ComposeStudentBasicData(IContainer container)
    //    {
    //        ComposeSection(container, "بيانات الطالب الأساسية", body =>
    //        {
    //            var items = new (string Label, string Value)[]
    //            {
    //            ("الاسم الكامل", _model.Student.FullName),
    //            ("تاريخ الميلاد", _model.Student.BirthDate.ToString("yyyy/MM/dd")),
    //            ("العمر", _model.Student.AgeText),
    //            ("الجنسية", _model.Student.Nationality),
    //            ("الحالة الصحية", _model.Student.HealthStatus),
    //            ("عدد الإخوة", _model.Student.SiblingsCount.ToString())
    //            };

    //            body.Table(table =>
    //            {
    //                table.ColumnsDefinition(cols =>
    //                {
    //                    cols.ConstantColumn(92);
    //                    cols.RelativeColumn();
    //                });

    //                foreach (var item in items)
    //                {
    //                    table.Cell().Element(LabelCell).Text(item.Label).SemiBold();
    //                    table.Cell().Element(ValueCell).Text(item.Value);
    //                }
    //            });
    //        });
    //    }

    //    private void ComposeStudentQuickInfo(IContainer container)
    //    {
    //        container.PaddingTop(14).Column(col =>
    //        {
    //            col.Spacing(8);
    //            col.Item().Text(_model.Student.FullName).FontSize(15).Bold().FontColor("#111827");
    //            col.Item().Element(Divider);
    //            col.Item().Element(c => QuickInfo(c, "الصف", _model.Academic.Grade));
    //            col.Item().Element(c => QuickInfo(c, "الحالة", _model.Student.Status, true));
    //        });
    //    }

    //    private void ComposeStudentPhoto(IContainer container)
    //    {
    //        container.Height(162).CornerRadius(8).Background("#F2F4F7").Clip().Element(c =>
    //        {
    //            if (!string.IsNullOrWhiteSpace(_model.StudentPhotoPath) && File.Exists(_model.StudentPhotoPath))
    //                c.Image(_model.StudentPhotoPath).FitArea();
    //            else
    //                c.AlignCenter().AlignMiddle().Text("صورة الطالب").FontSize(11).FontColor(Muted);
    //        });
    //    }

    //    private void ComposeAcademicData(IContainer container)
    //    {
    //        container.PaddingVertical(4).Row(row =>
    //        {
    //            AcademicItem(row, "العام الدراسي", _model.Academic.AcademicYear);
    //            AcademicItem(row, "المرحلة", _model.Academic.Stage);
    //            AcademicItem(row, "الصف", _model.Academic.Grade);
    //            AcademicItem(row, "تاريخ القيد", _model.Academic.RegistrationDate.ToString("yyyy/MM/dd"));
    //        });
    //    }

    //    private void ComposeGuardian(IContainer container)
    //    {
    //        KeyValueTable(container, new[]
    //        {
    //        ("الاسم", _model.Guardian.Name),
    //        ("صلة القرابة", _model.Guardian.Relationship),
    //        ("رقم الهاتف", _model.Guardian.Phone)
    //    });
    //    }

    //    private void ComposeContact(IContainer container)
    //    {
    //        KeyValueTable(container, new[]
    //        {
    //        ("العنوان الكامل", _model.Contact.FullAddress),
    //        ("رقم الهاتف", _model.Contact.Phone),
    //        ("البريد الإلكتروني", _model.Contact.Email)
    //    });
    //    }

    //    private void ComposeSecondPage(IContainer container)
    //    {
    //        container.Column(column =>
    //        {
    //            column.Spacing(9);

    //            column.Item().Element(ComposeFeeSummary);

    //            column.Item().Row(row =>
    //            {
    //                row.RelativeItem(1.25f).Element(c => ComposeSection(c, "تفاصيل الرسوم", ComposeFeesTable));
    //                row.ConstantItem(10);
    //                row.RelativeItem(1f).Element(c => ComposeSection(c, "آخر الأنشطة", ComposeActivities));
    //            });

    //            column.Item().Element(c => ComposeSection(c, "ملاحظات عامة", ComposeGeneralNotes));
    //        });
    //    }

    //    private void ComposeFeeSummary(IContainer container)
    //    {
    //        container.Column(col =>
    //        {
    //            col.Item().AlignRight().Text("ملخص الرسوم").FontSize(10).SemiBold().FontColor(NavyDark);
    //            col.Item().PaddingTop(5).Row(row =>
    //            {
    //                SummaryCard(row, "حالة السداد", _model.Fees.PaymentStatus, Red, RedBg);
    //                SummaryCard(row, "المتبقي", Money(_model.Fees.Remaining), Orange, OrangeBg);
    //                SummaryCard(row, "إجمالي الرسوم", Money(_model.Fees.Total), Navy, BlueBg);
    //                SummaryCard(row, "إجمالي المدفوع", Money(_model.Fees.Paid), Green, GreenBg);
    //            });
    //        });
    //    }

    //    private void ComposeFeesTable(IContainer container)
    //    {
    //        container.Table(table =>
    //        {
    //            table.ColumnsDefinition(cols =>
    //            {
    //                cols.ConstantColumn(28);
    //                cols.RelativeColumn(1.7f);
    //                cols.RelativeColumn();
    //                cols.RelativeColumn();
    //                cols.RelativeColumn();
    //                cols.RelativeColumn();
    //                cols.ConstantColumn(52);
    //            });

    //            foreach (var h in new[] { "#", "نوع الرسوم", "القيمة", "تاريخ الاستحقاق", "المبلغ المدفوع", "المتبقي", "الحالة" })
    //                table.Cell().Element(TableHeaderCell).Text(h).SemiBold();

    //            foreach (var item in _model.Fees.Items)
    //            {
    //                table.Cell().Element(TableBodyCell).Text(item.Number.ToString());
    //                table.Cell().Element(TableBodyCell).Text(item.FeeType);
    //                table.Cell().Element(TableBodyCell).Text(Money(item.Amount));
    //                table.Cell().Element(TableBodyCell).Text(item.DueDate.ToString("yyyy/MM/dd"));
    //                table.Cell().Element(TableBodyCell).Text(Money(item.PaidAmount));
    //                table.Cell().Element(TableBodyCell).Text(Money(item.RemainingAmount));
    //                table.Cell().Element(TableBodyCell).AlignCenter().Element(c => StatusBadge(c, item.Status));
    //            }
    //        });
    //    }

    //    private void ComposeActivities(IContainer container)
    //    {
    //        container.Table(table =>
    //        {
    //            table.ColumnsDefinition(cols =>
    //            {
    //                cols.ConstantColumn(74);
    //                cols.RelativeColumn();
    //                cols.ConstantColumn(54);
    //            });

    //            foreach (var h in new[] { "التاريخ", "النشاط", "النوع" })
    //                table.Cell().Element(TableHeaderCell).Text(h).SemiBold();

    //            foreach (var item in _model.Activities)
    //            {
    //                table.Cell().Element(TableBodyCell).Text(item.Date.ToString("yyyy/MM/dd"));
    //                table.Cell().Element(TableBodyCell).Text(item.Description);
    //                table.Cell().Element(TableBodyCell).AlignCenter().Element(c => ActivityBadge(c, item.Type));
    //            }
    //        });
    //    }

    //    private void ComposeGeneralNotes(IContainer container)
    //    {
    //        container.Padding(8).Column(col =>
    //        {
    //            col.Spacing(5);
    //            foreach (var note in _model.GeneralNotes)
    //            {
    //                col.Item().Row(row =>
    //                {
    //                    row.AutoItem().PaddingTop(2).Text("•").FontSize(12).FontColor(Navy);
    //                    row.RelativeItem().PaddingRight(5).Text(note).FontSize(9.5f);
    //                });
    //            }
    //        });
    //    }

    //    private void ComposeFooter(IContainer container, int pageNumber)
    //    {
    //        container.Background(NavyDark).PaddingHorizontal(14).Row(row =>
    //        {
    //            row.RelativeItem().AlignMiddle().Text($"تاريخ إصدار التقرير: {_model.ReportDate:dd/MM/yyyy}")
    //                .FontSize(7.5f).FontColor(Colors.White);

    //            row.ConstantItem(80).AlignCenter().AlignMiddle().Background(Colors.White).CornerRadius(8)
    //                .Text($"{pageNumber} / 2").FontSize(10).Bold().FontColor(NavyDark).ContentFromLeftToRight();

    //            row.RelativeItem().AlignLeft().AlignMiddle().Text($"{_model.School.Address}    |    {_model.School.Phone}")
    //                .FontSize(7.2f).FontColor(Colors.White);
    //        });
    //    }

    //    private static void ComposeSection(IContainer container, string title, Action<IContainer> body)
    //    {
    //        container.Border(1).BorderColor(Border).CornerRadius(7).Column(col =>
    //        {
    //            col.Item().AlignRight().PaddingRight(8).PaddingTop(5).Element(c =>
    //            {
    //                c.AlignRight().Background(NavyDark).CornerRadius(5).PaddingHorizontal(16).PaddingVertical(5)
    //                    .Text(title).FontSize(10.5f).SemiBold().FontColor(Colors.White);
    //            });

    //            col.Item().Padding(7).Element(body);
    //        });
    //    }

    //    private static void KeyValueTable(IContainer container, IEnumerable<(string Label, string Value)> rows)
    //    {
    //        container.Table(table =>
    //        {
    //            table.ColumnsDefinition(cols =>
    //            {
    //                cols.ConstantColumn(95);
    //                cols.RelativeColumn();
    //            });

    //            foreach (var row in rows)
    //            {
    //                table.Cell().Element(LabelCell).Text(row.Label).SemiBold();
    //                table.Cell().Element(ValueCell).Text(row.Value);
    //            }
    //        });
    //    }

    //    private static void QuickInfo(IContainer container, string label, string value, bool badge = false)
    //    {
    //        container.Column(col =>
    //        {
    //            col.Item().Text(label).FontSize(7.5f).FontColor(Muted);
    //            if (badge)
    //            {
    //                col.Item().PaddingTop(2).AlignRight().Background(GreenBg).CornerRadius(8).PaddingHorizontal(10).PaddingVertical(3)
    //                    .Text(value).FontSize(8.5f).SemiBold().FontColor(Green);
    //            }
    //            else
    //            {
    //                col.Item().PaddingTop(2).Text(value).FontSize(9).SemiBold();
    //            }
    //        });
    //    }

    //    private static void AcademicItem(RowDescriptor row, string label, string value)
    //    {
    //        row.RelativeItem().BorderLeft(1).BorderColor(Border).PaddingHorizontal(8).AlignCenter().Column(col =>
    //        {
    //            col.Item().AlignCenter().Text(label).FontSize(7.5f).SemiBold().FontColor(Navy);
    //            col.Item().PaddingTop(4).AlignCenter().Text(value).FontSize(8.5f);
    //        });
    //    }

    //    private static void SummaryCard(RowDescriptor row, string label, string value, string accent, string background)
    //    {
    //        row.RelativeItem().PaddingHorizontal(4).Border(1).BorderColor(accent).CornerRadius(6).Background(background)
    //            .PaddingVertical(10).PaddingHorizontal(12).Column(col =>
    //            {
    //                col.Item().AlignCenter().Text(label).FontSize(9).SemiBold().FontColor(accent);
    //                col.Item().PaddingTop(5).AlignCenter().Text(value).FontSize(12).Bold().FontColor(NavyDark);
    //            });
    //    }

    //    private static void StatusBadge(IContainer container, string status)
    //    {
    //        var isPaid = status.Contains("مدفوع", StringComparison.OrdinalIgnoreCase);
    //        container.Background(isPaid ? GreenBg : RedBg).CornerRadius(7).PaddingHorizontal(7).PaddingVertical(2)
    //            .Text(status).FontSize(7.5f).SemiBold().FontColor(isPaid ? Green : Red);
    //    }

    //    private static void ActivityBadge(IContainer container, string type)
    //    {
    //        var (bg, fg) = type switch
    //        {
    //            "حضور" => (GreenBg, Green),
    //            "سداد" => (BlueBg, Navy),
    //            "نتيجة" => (PurpleBg, "#6B46C1"),
    //            _ => (OrangeBg, Orange)
    //        };

    //        container.Background(bg).CornerRadius(7).PaddingHorizontal(6).PaddingVertical(2)
    //            .Text(type).FontSize(7.3f).SemiBold().FontColor(fg);
    //    }

    //    private static IContainer LabelCell(IContainer container) =>
    //        container.BorderBottom(1).BorderColor(Border).Background(LightBlue).PaddingHorizontal(7).PaddingVertical(5).AlignMiddle();

    //    private static IContainer ValueCell(IContainer container) =>
    //        container.BorderBottom(1).BorderColor(Border).PaddingHorizontal(7).PaddingVertical(5).AlignMiddle();

    //    private static IContainer TableHeaderCell(IContainer container) =>
    //        container.Border(1).BorderColor(Border).Background(LightBlue).PaddingHorizontal(4).PaddingVertical(5).AlignCenter().AlignMiddle();

    //    private static IContainer TableBodyCell(IContainer container) =>
    //        container.Border(1).BorderColor(Border).PaddingHorizontal(4).PaddingVertical(5).AlignCenter().AlignMiddle();

    //    private static void Divider(IContainer container) => container.Height(1).Background(Border);

    //    private static string Money(decimal value) => $"{value:N2} ج.م";

    //    private const string MortarboardSvg = """
    //    <svg viewBox="0 0 64 40" xmlns="http://www.w3.org/2000/svg">
    //      <path fill="#073B78" d="M32 2 3 15l29 13 24-10.8V30h4V15L32 2Z"/>
    //      <path fill="#073B78" d="M15 22v8c0 5 8 8 17 8s17-3 17-8v-8l-17 8-17-8Z"/>
    //    </svg>
    //    """;
    //}
}
