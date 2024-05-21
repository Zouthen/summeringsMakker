using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace summeringsMakker.Services;

public class TextExtractor
{
    public static string ExtractTextFromPdf(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfPage page = pdfDoc.GetPage(i);
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    text.AppendLine(pageText);
                }

                return text.ToString();
            }
        }

}
