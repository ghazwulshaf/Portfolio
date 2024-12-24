using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers;

public class FileController: Controller
{
    public IActionResult DownloadFile(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found!");
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, "application/octet-stream", fileName);
    }
}