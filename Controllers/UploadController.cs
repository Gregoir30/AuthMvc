using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace AuthMvc.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier sélectionné.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "upload", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { filePath = "/upload/" + file.FileName });
        }
    }
}
