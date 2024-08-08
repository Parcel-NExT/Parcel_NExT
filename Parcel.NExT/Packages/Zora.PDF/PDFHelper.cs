using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Zora.FileFormats
{
    public static class PDFHelper
    {
        /// <summary>
        /// Extract raw texts for LLM use purpose.
        /// 
        /// Notice output is not formatted and may not be directly usable other than as LLM input.
        /// </summary>
        public static string GetTexts(string pdfFile)
        {
            StringBuilder body = new();

            int pageNumber = 0;
            using PdfDocument document = PdfDocument.Open(@"C:\Documents\document.pdf");
            foreach (Page page in document.GetPages())
            {
                string pageText = page.Text;
                
                body.AppendLine($"Page {pageNumber}");
                body.AppendLine(pageText);

                pageNumber++;
            }

            return body.ToString().TrimEnd();
        }

        public static void WritePDF(string title, string body, string outputPath)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text(title)
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text(body);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(outputPath);
        }
    }
}
