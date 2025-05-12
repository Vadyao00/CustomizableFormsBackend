namespace Contracts.IServices;

public interface IDropboxService
{
    Task<string> UploadJsonFileAsync(string json, string fileName);
}