namespace Contracts.IServices;

public interface IOneDriveService
{
    Task<string> UploadJsonFileAsync(string json, string fileName);
}