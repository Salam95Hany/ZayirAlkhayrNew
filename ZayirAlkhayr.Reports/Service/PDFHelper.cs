using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.Layout.Font;
using iText.Kernel.Geom;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using ZayirAlkhayr.Reports.Interface;

namespace ZayirAlkhayr.Reports.Service
{
    public class PDFHelper: IPDFHelper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly PdfFont _pdfFont;

        public PDFHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
            var fontPath = System.IO.Path.Combine(_environment.WebRootPath, "Fonts", "Cairo-Regular.ttf");
            _pdfFont = PdfFontFactory.CreateFont(fontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);
        }

        public string SaveHTMLResult(string HTMLContent, bool IsLandScape)
        {
            try
            {
                HTMLContent = ClearAngularAttrFromHTML(HTMLContent);
                var FolderPath = System.IO.Path.Combine(_environment.WebRootPath, "Reports");

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                var FilePath = System.IO.Path.Combine(FolderPath, "Report_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf");
                ConvertHtmlToPdf(HTMLContent, FilePath, IsLandScape);

                return FilePath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ConvertHtmlToPdf(string HTMLContent, string outputPath, bool IsLandScape)
        {
            try
            {
                var html = @"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <style>
                                @page {
                                    margin: 5px !important;
                                    padding: 0 !important;
                                }
                                html, body {
                                    margin: 0 !important;
                                    padding: 0 !important;
                                    width: 100%;
                                    height: 100%;
                                }
                                * {
                                    box-sizing: border-box;
                                    margin: 0;
                                    padding: 0;
                                }
                                body {
                                    -webkit-print-color-adjust: exact;
                                    print-color-adjust: exact;
                                }
                            </style>
                        </head>
                        <body>
                            {HTMLContent}
                        </body>
                        </html>";
                HTMLContent = html.Replace("{HTMLContent}", HTMLContent);
                string tempFile = System.IO.Path.GetTempFileName();
                WriterProperties writerProperties = new WriterProperties().SetFullCompressionMode(true);
                using (FileStream pdfStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                using (PdfWriter writer = new PdfWriter(pdfStream, writerProperties))
                using (PdfDocument pdfDocument = new PdfDocument(writer))
                {
                    if (!IsLandScape)
                        pdfDocument.SetDefaultPageSize(PageSize.A4.Rotate());

                    FontProvider fontProvider = new FontProvider();
                    fontProvider.AddFont(_pdfFont.GetFontProgram());
                    ConverterProperties properties = new ConverterProperties();
                    properties.SetCharset("UTF-8");
                    properties.SetFontProvider(fontProvider);
                    Document document = HtmlConverter.ConvertToDocument(HTMLContent, pdfDocument, properties);
                    document.SetMargins(0, 0, 0, 0);
                    int pageCount = pdfDocument.GetNumberOfPages();
                    for (int i = 1; i <= pageCount; i++)
                    {
                        if (i % 5000 == 0)
                        {
                            pdfDocument.GetPage(i).Flush();
                        }
                    }
                    document.Close();
                }
                using (PdfReader reader = new PdfReader(tempFile))
                using (PdfWriter finalWriter = new PdfWriter(outputPath))
                using (PdfDocument finalPdfDocument = new PdfDocument(reader, finalWriter))
                {
                    AddFooter(finalPdfDocument);
                    finalPdfDocument.Close();
                }
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AddFooter(PdfDocument pdfDocument)
        {
            int numberOfPages = pdfDocument.GetNumberOfPages();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

            for (int i = 1; i <= numberOfPages; i++)
            {
                PdfPage page = pdfDocument.GetPage(i);
                PdfCanvas canvas = new PdfCanvas(page);

                float pageWidth = page.GetPageSize().GetWidth();
                float margin = 40;
                float marginRight = 150;
                float y = 20;

                string pageNumberText = $"Page {i} of {numberOfPages}";
                canvas.BeginText()
                    .SetFontAndSize(_pdfFont, 10)
                    .SetColor(ColorConstants.BLACK, true)
                    .MoveText(margin, y)
                    .ShowText(pageNumberText)
                    .EndText();

                canvas.BeginText()
                    .SetFontAndSize(_pdfFont, 10)
                    .SetColor(ColorConstants.BLACK, true)
                    .MoveText(pageWidth - marginRight, y)
                    .ShowText(currentDate)
                    .EndText();

                canvas.Release();
            }
        }

        public string ClearAngularAttrFromHTML(string HTML)
        {
            try
            {
                if (string.IsNullOrEmpty(HTML))
                    return HTML;

                HTML = Regex.Replace(HTML, "( _nghost-ng-cli-universal-c| _ngcontent-ng-cli-universal-c)[1-9]*=\"\"", "");
                HTML = Regex.Replace(HTML, "<!--([a-z]+)(?![^>]*\\/>)[^>]*-->", "");
                HTML = Regex.Replace(HTML, @"\s_ngcontent-[a-zA-Z0-9\-]+?=""[^""]*""", "");

                return HTML;
            }
            catch (Exception)
            {
                return HTML;
            }
        }
    }
}
