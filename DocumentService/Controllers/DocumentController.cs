using Microsoft.AspNetCore.Mvc;
using DocumentService.Data;
using DocumentService.DTOs;
using DocumentService.Models;
using DocumentService.Services;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPdfGeneratorService _pdfGenerator;
        private readonly IWebHostEnvironment _environment;

        public DocumentController(
            AppDbContext context, 
            IPdfGeneratorService pdfGenerator,
            IWebHostEnvironment environment)
        {
            _context = context;
            _pdfGenerator = pdfGenerator;
            _environment = environment;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<DocumentResponse>> GenerateDocument([FromBody] GenerateDocumentRequest request)
        {
            try
            {
                // Generate PDF
                var fileName = await _pdfGenerator.GenerateLicensePdfAsync(request);
                var documentPath = Path.Combine("GeneratedDocuments", fileName);

                // Save document record
                var document = new Document
                {
                    LicenseId = request.LicenseId,
                    LicenseNumber = request.LicenseNumber,
                    DocumentPath = documentPath,
                    FileName = fileName,
                    TenantId = request.TenantId,
                    GeneratedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Document generated successfully",
                    DocumentId = document.Id,
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Document generation failed: {ex.Message}"
                });
            }
        }

        [HttpGet("download/{licenseId}")]
        public async Task<ActionResult> DownloadLicense(int licenseId, [FromQuery] int tenantId)
        {
            var document = await _context.Documents
                .Where(d => d.LicenseId == licenseId && d.TenantId == tenantId)
                .OrderByDescending(d => d.GeneratedDate)
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return NotFound(new { message = "Document not found" });
            }

            var filePath = Path.Combine(_environment.ContentRootPath, document.DocumentPath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "File not found on server" });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", document.FileName);
        }

        [HttpGet("info/{licenseId}")]
        public async Task<ActionResult> GetDocumentInfo(int licenseId, [FromQuery] int tenantId)
        {
            var document = await _context.Documents
                .Where(d => d.LicenseId == licenseId && d.TenantId == tenantId)
                .OrderByDescending(d => d.GeneratedDate)
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return NotFound(new { message = "Document not found" });
            }

            return Ok(new
            {
                documentId = document.Id,
                licenseNumber = document.LicenseNumber,
                fileName = document.FileName,
                generatedDate = document.GeneratedDate
            });
        }
    }
}