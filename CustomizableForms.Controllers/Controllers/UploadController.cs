using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contracts.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CustomizableForms.Controllers.Controllers;

[Route("api/upload")]
[ApiController]
public class UploadController : ApiControllerBase
{
    private readonly Cloudinary _cloudinary;
    
    public UploadController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor,IConfiguration configuration)
     : base (serviceManager, httpContextAccessor)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }
    
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Cannot find file");

        try
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "templates",
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return Ok(new
            {
                ImageUrl = uploadResult.SecureUrl.ToString()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error while uploading image: {ex.Message}");
        }
    }
}