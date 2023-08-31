namespace Agorus.Data.Models
{
    public class File
    {
        public int Id { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public int Version { get; set; }
    }
}