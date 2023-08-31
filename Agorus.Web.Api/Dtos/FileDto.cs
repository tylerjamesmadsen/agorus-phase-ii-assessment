namespace Agorus.Web.Api.Dtos
{
    public class FileDto
    {
        public int Id { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int Version { get; set; }

        public static explicit operator FileDto(Data.Models.File file)
        {
            return new FileDto
            {
                Id = file.Id,
                FileId = file.FileId,
                Version = file.Version,
                FileName = file.FileName,
            };
        }
    }
}
