using DocumentService.DTOs;

namespace DocumentService.Services
{
    public interface IPdfGeneratorService
    {
        Task<string> GenerateLicensePdfAsync(GenerateDocumentRequest request);
    }
}