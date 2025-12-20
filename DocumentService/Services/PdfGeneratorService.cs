using Microsoft.AspNetCore.Hosting;

using iTextSharp.text;
using iTextSharp.text.pdf;
using DocumentService.DTOs;

namespace DocumentService.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;

        public PdfGeneratorService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> GenerateLicensePdfAsync(GenerateDocumentRequest request)
        {
            var documentsPath = Path.Combine(_environment.ContentRootPath, "GeneratedDocuments");
            
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            var fileName = $"License_{request.LicenseNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(documentsPath, fileName);

            await Task.Run(() =>
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);

                    document.Open();

                    // Add header
                    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DarkGray);
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.Black);
                    Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.Black);
                    Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.Black);

                    // Title
                    Paragraph title = new Paragraph("PROFESSIONAL LICENSE CERTIFICATE", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20;
                    document.Add(title);

                    // License Number
                    Paragraph licenseNum = new Paragraph($"License Number: {request.LicenseNumber}", headerFont);
                    licenseNum.Alignment = Element.ALIGN_CENTER;
                    licenseNum.SpacingAfter = 30;
                    document.Add(licenseNum);

                    // Separator line
                    iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Gray, Element.ALIGN_CENTER, -1);
                    document.Add(new Chunk(line));
                    document.Add(new Paragraph(" "));

                    // License details table
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 40f, 60f });
                    table.SpacingBefore = 20;
                    table.SpacingAfter = 20;

                    AddTableRow(table, "Applicant Name:", request.ApplicantName, labelFont, normalFont);
                    AddTableRow(table, "Company Name:", request.CompanyName, labelFont, normalFont);
                    AddTableRow(table, "GST Number:", request.GstNumber, labelFont, normalFont);
                    AddTableRow(table, "City:", request.City, labelFont, normalFont);
                    AddTableRow(table, "Approval Date:", request.ApprovalDate.ToString("dd-MMM-yyyy"), labelFont, normalFont);
                    AddTableRow(table, "Expiry Date:", request.ExpiryDate.ToString("dd-MMM-yyyy"), labelFont, normalFont);

                    document.Add(table);

                    // Footer text
                    document.Add(new Paragraph(" "));
                    document.Add(new Chunk(line));
                    document.Add(new Paragraph(" "));

                    Paragraph footer = new Paragraph(
                        "This is a computer-generated license certificate. " +
                        "It is valid for one year from the date of approval. " +
                        "Please renew before expiry date.",
                        FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9, BaseColor.DarkGray)
                    );
                    footer.Alignment = Element.ALIGN_CENTER;
                    document.Add(footer);

                    // Generated date
                    Paragraph generated = new Paragraph(
                        $"Generated on: {DateTime.UtcNow:dd-MMM-yyyy HH:mm:ss} UTC",
                        FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.LightGray)
                    );
                    generated.Alignment = Element.ALIGN_RIGHT;
                    generated.SpacingBefore = 30;
                    document.Add(generated);

                    document.Close();
                }
            });

            return fileName;
        }

        private void AddTableRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.PaddingBottom = 10;
            labelCell.PaddingTop = 5;

            PdfPCell valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.PaddingBottom = 10;
            valueCell.PaddingTop = 5;

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }
    }
}