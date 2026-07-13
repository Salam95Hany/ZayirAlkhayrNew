using Microsoft.Net.Http.Headers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Globalization;
using ZayirAlkhayr.Entities.Reports;

namespace ZayirAlkhayr.Reports.Service
{
    public static class PdfFactory
    {
        public static void AddHeaderContent(this IContainer Container, string ImgPath, string Header, string HeaderDetails)
        {
            Container.Column(col =>
            {
                col.Item().BackgroundLinearGradient(135,
                    new Color[]
                    {
                    Color.FromHex("#2d5016"),
                    Color.FromHex("#4a7c23")
                    }).Padding(5).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("مؤسسة زائر الخیر")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.White);

                            c.Item().Text("المشھرة برقم 2424 لسنة 2424")
                                .FontSize(14)
                                .FontColor(Colors.White.WithAlpha(0.9f));

                            c.Item().Text("تحت رعایة وزارة التضامن الاجتماعي")
                                .FontSize(14)
                                .FontColor(Colors.White.WithAlpha(0.9f));
                        });

                        row.ConstantItem(70)
                           .Height(70)
                           .Width(70)
                           .Border(3, Colors.White.WithAlpha(0.3f))
                           .CornerRadius(35)
                           .AlignCenter()
                           .Image(ImgPath, ImageScaling.FitArea);
                    });

                col.Item().PaddingVertical(5).AlignCenter().Width(250).Element(container2 =>
                {
                    container2
                        .BorderRight(5)
                        .BorderLeft(5)
                        .BorderColor(Color.FromHex("#4a7c23"))
                        .BackgroundLinearGradient(
                            135,
                            new[]
                            {
                            Color.FromHex("#f8fdf8"),
                            Color.FromHex("#e8f5e8")
                            })
                        .CornerRadius(12)
                        .AlignCenter()
                        .Column(inner =>
                        {
                            inner.Item().Text(Header)
                                .FontSize(28)
                                .Bold()
                                .FontColor(Color.FromHex("#2d5016"))
                                .AlignCenter();

                            inner.Item().Text(HeaderDetails)
                                .FontSize(16)
                                .FontColor(Color.FromHex("#4a7c23"))
                                .AlignCenter();
                        });
                });
            });
        }

        public static void AddTableContent(this IContainer Container, DataTable dt, List<PDFHeaderSelected> HeaderNames)
        {
            Container.Column(col =>
            {
                col.Item().PaddingVertical(5).Table(tbl =>
                {
                    IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                    {
                        return container
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .Background(backgroundColor)
                            .PaddingVertical(5)
                            .PaddingHorizontal(5);
                    }

                    tbl.ColumnsDefinition(columns =>
                    {
                        for (int i = 0; i < HeaderNames.Count; i++)
                            columns.RelativeColumn();
                    });

                    tbl.Header(header =>
                    {
                        foreach (var name in HeaderNames)
                        {
                            header.Cell().Element(HeaderCellStyle).Text(name.NameAr);
                        }
                        IContainer HeaderCellStyle(IContainer container) => container
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .BackgroundLinearGradient(
                            135,
                            new[]
                            {
                                Color.FromHex("#f8fdf8"),
                                Color.FromHex("#e8f5e8")
                            })
                        .PaddingVertical(5)
                        .PaddingHorizontal(5);
                    });
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        foreach (var column in HeaderNames)
                        {
                            var cellValue = dt.Rows[i][column.NameEn]?.ToString() ?? "";
                            tbl.Cell().Element(CellStyle).Text(cellValue);
                        }
                    }

                    IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);
                });
            });
        }

        public static void AddFooterContent(this IContainer Container)
        {
            string currentDate = DateTime.UtcNow.EgyptNow().ToString("d MMMM , yyyy hh:mm t", new CultureInfo("ar-AE"));

            Container.Height(25).BackgroundLinearGradient(135,
                    new Color[]
                    {
                    Color.FromHex("#2d5016"),
                    Color.FromHex("#4a7c23")
                    }).PaddingVertical(5)
                .Row(row =>
                {
                    row.RelativeItem().PaddingLeft(70).AlignLeft().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.White));
                        text.Span("📄 الصفحة ");
                        text.CurrentPageNumber();
                        text.Span(" من ");
                        text.TotalPages();
                    });

                    row.RelativeItem();
                    row.RelativeItem();

                    row.RelativeItem().PaddingRight(20).AlignRight().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.White));
                        text.Span(" 📅 " + currentDate);
                    });
                });
        }

        public static void AddTotalAmountCard(this IContainer container, string label, string amount, string currency)
        {
            container.PaddingTop(10).Element(card =>
            {
                card
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .BackgroundLinearGradient(
                    135,
                    new[]
                    {
                Color.FromHex("#f8fdf8"),
                Color.FromHex("#e8f5e8")
                    })
                .CornerRadius(8)
                .Padding(10)
                .Row(row =>
                {
                    row.RelativeItem().AlignMiddle().Text(text =>
                    {
                        text.Span("💰 ").FontSize(18);
                        text.Span(label)
                            .FontSize(20)
                            .FontColor(Color.FromHex("#2d5016"))
                            .Bold();
                    });

                    row.AutoItem().AlignMiddle().Row(inner =>
                    {
                        inner.AutoItem().Text(amount)
                            .FontSize(26)
                            .Bold()
                            .FontColor(Color.FromHex("#2d5016"));

                        inner.AutoItem().PaddingLeft(6).AlignMiddle().Text(currency)
                            .FontSize(18)
                            .FontColor(Color.FromHex("#4a7c23"));
                    });
                });
            });
        }
    }

}
