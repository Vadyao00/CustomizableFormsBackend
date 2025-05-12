using System.Text;
using Contracts.IServices;
using CustomizableForms.LoggerService;
using Microsoft.Extensions.Configuration;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace CustomizableForms.Application.Services;

public class DropboxService : IDropboxService
{
    private readonly ILoggerManager _logger;
    private readonly IConfiguration _configuration;

    public DropboxService(ILoggerManager logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> UploadJsonFileAsync(string json, string fileName)
    {
        try
        {
            string accessToken = _configuration["Dropbox:AccessToken"];
            
            using var dropboxClient = new DropboxClient(accessToken);
            
            string folderPath = "/support-tickets";
            
            byte[] fileContent = Encoding.UTF8.GetBytes(json);
            using var stream = new MemoryStream(fileContent);
            
            string fullPath = $"{folderPath}/{fileName}";
            if (!fullPath.StartsWith("/"))
                fullPath = $"/{fullPath}";
            
            var result = await dropboxClient.Files.UploadAsync(
                path: fullPath,
                mode: WriteMode.Overwrite.Instance,
                body: stream);
            
            var sharedLink = await dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(
                path: result.PathDisplay);
            
            _logger.LogInfo($"File '{fileName}' uploaded successfully to Dropbox. Path: {result.PathDisplay}");
            return sharedLink.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file '{fileName}' to Dropbox: {ex.Message}");
            Console.WriteLine($"------ {ex.Message}");
            throw;
        }
    }
}