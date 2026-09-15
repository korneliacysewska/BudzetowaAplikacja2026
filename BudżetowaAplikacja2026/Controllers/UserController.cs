using Microsoft.AspNetCore.Mvc;
using BudżetowaAplikacja2026.Services;

namespace BudżetowaAplikacja2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly BlobService _blobService;

        public UserController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Brak pliku do przesłania.");
            }

            var uri = await _blobService.UploadProfilePicture(file);
            return Ok(new { Url = uri });
        }
    }

}
