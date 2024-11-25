namespace TaskManagementSystem.FileUpload.Service
{
    using System.IO;
    using System.Threading.Tasks;
    using TaskManagementSystem.FileUpload;

    namespace TaskManagementSystem.FileUpload
    {
        public interface IFileService
        {
            // Method to upload a single file asynchronously
            Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType);
            // Additional methods can be added if needed, such as validation methods or file metadata extraction
        }
    }

}
