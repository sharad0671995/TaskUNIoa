namespace TaskManagementSystem.FileUpload
{
    public class FileUploadSummary
    {

        public int FileCount { get; set; }
        public long TotalSizeInBytes { get; set; }
        public string TotalSizeFormatted { get; set; }
        public List<string> UploadedFilePaths { get; set; }
        public List<string> NotUploadedFiles { get; set; }
    }
}
