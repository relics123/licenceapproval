namespace DocumentService.DTOs
{
    public class DocumentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int DocumentId { get; set; }
        public string FileName { get; set; }
    }
}