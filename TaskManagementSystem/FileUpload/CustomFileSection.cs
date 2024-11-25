namespace TaskManagementSystem.FileUpload
{
    public class CustomFileSection
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Stream FileStream { get; set; }

        // Constructor
        public CustomFileSection(string fileName, string contentType, Stream fileStream)
        {
            FileName = fileName;
            ContentType = contentType;
            FileStream = fileStream;
        }
    }

}
