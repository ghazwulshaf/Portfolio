using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Dashboard/About")]
    public class DashboardAboutController : Controller
    {
        private readonly string _profileJsonFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "profile.json");
        private readonly string _profilePhotoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile");
        private readonly AboutProfileService _profileService;

        public DashboardAboutController(AboutProfileService profileService)
        {
            _profileService = profileService;

            // pastikan setiap folder tersedia
            if (!Directory.Exists(_profilePhotoFolder))
            {
                Directory.CreateDirectory(_profilePhotoFolder);
            }
        }

        // GET: DashboardAboutController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("/Views/Admin/Dashboard/About/Index.cshtml",
                new About {
                    Profile = await _profileService.GetAsync()
                }
            );
        }

        [HttpPost]
        [Route("Update")]
        // [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Update(About About)
        {
            if (About.Profile.Data != null)
            {
                // simpan file gambar jika terdapat file terupload
                if (About.ProfilePhoto.File != null && About.ProfilePhoto.File.Length > 0)
                {
                    var photoFileName = Path.GetFileName(About.ProfilePhoto.File.FileName);
                    var photoFilePath = Path.Combine(_profilePhotoFolder, photoFileName);

                    // pastikan tidak terdapat file gambar dengan nama yang sama
                    if (System.IO.File.Exists(photoFilePath))
                    {
                        // hapus file gambar jika ada
                        System.IO.File.Delete(photoFilePath);
                    }

                    // simpan file gambar
                    using (var stream = new FileStream(photoFilePath, FileMode.Create))
                    {
                        await About.ProfilePhoto.File.CopyToAsync(stream);
                    }

                    // simpan data file gambar pada json
                    About.Profile.Data["Photo"] = "/images/profile/" + photoFileName;
                }

                // simpan data ke file
                await _profileService.UpdateAsync(About.Profile);

                // kembali ke halaman Index dan tampilkan pesan
                TempData["SuccessMessage"] = "Profile has been updated!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Profile updating was error!";
            return View("/Views/Admin/Dashboard/About/Index.cshtml", About);
        }

    }
}
