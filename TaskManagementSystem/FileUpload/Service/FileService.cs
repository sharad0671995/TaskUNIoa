using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using TaskManagementSystem.FileUpload.Service.TaskManagementSystem.FileUpload;

namespace TaskManagementSystem.FileUpload.Service
{
    public class FileService :IFileService
    {
        private const string UploadDirectory = "FileUpload";
        private readonly List<string> _allowedExtensions = new List<string> { ".zip", ".bin", ".png", ".jpg" };

        public async Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType)
        {
            var fileCount = 0;
            long totalSizeInBytes = 0;

            // Get the boundary for multipart/form-data parsing
            var boundary = GetBoundary(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType));

            // Create a multipart reader for the provided file stream
            var multipartReader = new MultipartReader(boundary, fileStream);
            var section = await multipartReader.ReadNextSectionAsync();

            var filePaths = new List<string>();
            var notUploadedFiles = new List<string>();

            // Iterate through all sections in the multipart data
            while (section != null)
            {
                // Check if the section is a file section
                var fileSection = section.AsFileSection();
                if (fileSection != null)
                {
                    try
                    {
                        // Save the file and get the size of the uploaded file
                        var result = await SaveFileAsync(fileSection, filePaths, notUploadedFiles);

                        // Only add to totals if the file was saved successfully
                        if (result > 0)
                        {
                            totalSizeInBytes += result;
                            fileCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error if something goes wrong during file saving
                        notUploadedFiles.Add(fileSection.FileName);
                        Console.WriteLine($"Error uploading file {fileSection.FileName}: {ex.Message}");
                    }
                }
                else
                {
                    // Optionally, log or handle non-file sections
                    Console.WriteLine("Non-file section encountered.");
                }

                // Move to the next section in the multipart data
                section = await multipartReader.ReadNextSectionAsync();
            }

            // Prepare and return the summary
            return new FileUploadSummary
            {
                FileCount = fileCount,
                TotalSizeInBytes = totalSizeInBytes,
                TotalSizeFormatted = ConvertSizeToString(totalSizeInBytes),
                UploadedFilePaths = filePaths,
                NotUploadedFiles = notUploadedFiles
            };
        }




        private async Task<int> SaveFileAsync(FileMultipartSection fileSection, List<string> filePaths, List<string> notUploadedFiles)
        {
            try
            {
                // Validate file extension
                var extension = Path.GetExtension(fileSection.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    // Log invalid extension and return 0
                    notUploadedFiles.Add(fileSection.FileName);
                    Console.WriteLine($"Invalid file extension for {fileSection.FileName}. Allowed extensions: {_allowedExtensions}");
                    return 0;
                }

                // Ensure the upload directory exists
                Directory.CreateDirectory(UploadDirectory);

                // Define the file path where the file will be saved
                var filePath = Path.Combine(UploadDirectory, fileSection.FileName);

                // Open a file stream to save the file
                await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024);

                // Copy the file content from the file stream to the target file stream
                await fileSection.FileStream.CopyToAsync(stream);

                // Log successful upload and add file path to the list
                filePaths.Add(filePath);
                Console.WriteLine($"File saved successfully: {filePath}");

                // Return the size of the uploaded file (in bytes)
                return (int)fileSection.FileStream.Length;
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during file saving
                Console.WriteLine($"Error saving file {fileSection.FileName}: {ex.Message}");

                // Add the file name to the failed uploads list
                notUploadedFiles.Add(fileSection.FileName);
                return 0; // Return 0 if the file could not be uploaded
            }
        }

        // Construct the full file path
        private string GetFullFilePath(CustomFileSection fileSection)
        {
            return !string.IsNullOrEmpty(fileSection.FileName)
                ? Path.Combine(Directory.GetCurrentDirectory(), UploadDirectory, fileSection.FileName)
                : string.Empty;
        }

        // Extract boundary from the content-type header
        private string GetBoundary(MediaTypeHeaderValue mediaTypeHeaderValue)
        {
            var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeaderValue.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
                throw new InvalidDataException("Missing content type boundary");

            return boundary;
        }

        // Convert file size to a human-readable format
        private string ConvertSizeToString(long byteSize)
        {
            if (byteSize < 1024)
                return byteSize + " B";
            else if (byteSize < 1024 * 1024)
                return (byteSize / 1024.0).ToString("0.##") + " KB";
            else if (byteSize < 1024 * 1024 * 1024)
                return (byteSize / (1024.0 * 1024.0)).ToString("0.##") + " MB";
            else if (byteSize < 1024L * 1024L * 1024L * 1024L)
                return (byteSize / (1024.0 * 1024.0 * 1024.0)).ToString("0.##") + " GB";
            else
                return (byteSize / (1024.0 * 1024.0 * 1024.0 * 1024.0)).ToString("0.##") + " TB";
        }

       
    }

    // Define a structure to hold the file upload summary
   
}
