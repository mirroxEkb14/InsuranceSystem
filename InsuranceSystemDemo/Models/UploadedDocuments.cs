namespace InsuranceSystemDemo.Models;

public class UploadedDocuments
{
    public int DocumentId { get; set; }
    public string? DocumentName { get; set; }
    public string? FileType { get; set; }
    public string? Extension { get; set; }
    public DateTime UploadDate { get; set; }
    public string? UploadedBy { get; set; }
}
