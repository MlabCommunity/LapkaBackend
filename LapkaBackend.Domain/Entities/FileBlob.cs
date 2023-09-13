namespace LapkaBackend.Domain.Entities
{
    public class FileBlob
    {
        public Guid Id { get; set; }
        public string UploadName { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public Guid ParentEntityId { get; set; }
        public int? Index { get; set; }
    }
}
