using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Dashboard/About")]
    public class DashboardAboutController : Controller
    {
        private readonly string _profilePhotoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile");
        private readonly string _cvFileFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
        private readonly AboutProfileService _profileService;
        private readonly AboutSectionService _sectionService;

        public DashboardAboutController(AboutProfileService profileService, AboutSectionService sectionsService)
        {
            _profileService = profileService;
            _sectionService = sectionsService;

            if (!Directory.Exists(_profilePhotoFolder))
                Directory.CreateDirectory(_profilePhotoFolder);

            if (!Directory.Exists(_cvFileFolder))
                Directory.CreateDirectory(_cvFileFolder);
        }

        // GET: DashboardAboutController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("/Views/Admin/Dashboard/About/Index.cshtml",
                new About {
                    Profile = await _profileService.GetAsync(),
                    Sections = await _sectionService.GetAllAsync()
                }
            );
        }

        // POST: Update About Profile
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(About About, IFormFile ProfilePhotoFile, IFormFile ProfileCVFile)
        {
            if (About.Profile != null)
            {
                if (ProfilePhotoFile != null && ProfilePhotoFile.Length > 0)
                {
                    var photoFileName = Path.GetFileName(ProfilePhotoFile.FileName);
                    var photoFilePath = Path.Combine(_profilePhotoFolder, photoFileName);

                    if (System.IO.File.Exists(photoFilePath))
                        System.IO.File.Delete(photoFilePath);

                    using var stream = new FileStream(photoFilePath, FileMode.Create);
                    await ProfilePhotoFile.CopyToAsync(stream);

                    About.Profile.Photo = "/images/profile/" + photoFileName;
                }

                if (ProfileCVFile != null && ProfileCVFile.Length > 0)
                {
                    var cvFileName = "curriculumvitae" + Path.GetExtension(ProfileCVFile.FileName);
                    var cvFilePath = Path.Combine(_cvFileFolder, cvFileName);

                    if (System.IO.File.Exists(cvFilePath))
                        System.IO.File.Delete(cvFilePath);

                    using var stream = new FileStream(cvFilePath, FileMode.Create);
                    await ProfileCVFile.CopyToAsync(stream);

                    About.Profile.CV = cvFileName;
                }

                await _profileService.UpdateAsync(About.Profile);

                TempData["SuccessMessage"] = "Profile has been updated!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Profile updating was error!";
            return View("/Views/Admin/Dashboard/About/Index.cshtml", About);
        }

        // GET: Add Section
        [HttpGet]
        [Route("Sections/Add")]
        public IActionResult AddSection()
        {
            ViewBag.Session = "Add";
            ViewBag.Action = "AddSection";

            var view = "/Views/Shared/Dashboard/About/_SectionForm.cshtml";
            var section = new AboutSection();

            return PartialView(view, section);
        }

        // POST: Add Section
        [HttpPost]
        [Route("Sections/Add")]
        public async Task<IActionResult> AddSection(AboutSection section)
        {
            await _sectionService.AddAsync(section);
            return RedirectToAction(nameof(Index));
        }

        // POST: Edit Section
        [HttpGet]
        [Route("Sections/{id}/Edit")]
        public async Task<IActionResult> EditSection(int id)
        {
            ViewBag.Session = "Edit";
            ViewBag.Action = "EditSection";

            var view = "/Views/Shared/Dashboard/About/_SectionForm.cshtml";
            var section = await _sectionService.GetByIdAsync(id);

            return PartialView(view, section);
        }

        // POST: Edit Section
        [HttpPost]
        [Route("Sections/{id}/Edit")]
        public async Task<IActionResult> EditSection(AboutSection section)
        {
            await _sectionService.UpdateAsync(section);
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Section
        [HttpPost]
        [Route("Sections/{id}/Delete")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            await _sectionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Add Section Item
        [HttpGet]
        [Route("Sections/{sectionId}/Add")]
        public async Task<IActionResult> AddSectionItem(int sectionId)
        {
            var view = "/Views/Shared/Dashboard/About/_SectionItem.cshtml";
            var section = await _sectionService.GetByIdAsync(sectionId);

            ViewBag.Id = section.Id;
            ViewBag.Title = section.Title;
            ViewBag.Session = "Add";
            ViewBag.Action = "AddSectionItem";

            return PartialView(view, new AboutSectionItem());
        }

        // POST: Add Section Item
        [HttpPost]
        [Route("Sections/{sectionId}/Add")]
        public async Task<IActionResult> AddSectionItem(int sectionId, AboutSectionItem item)
        {
            await _sectionService.AddItemAsync(sectionId, item);
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit Section Item
        [HttpGet]
        [Route("Sections/{sectionId}/{itemId}/Edit")]
        public async Task<IActionResult> EditSectionItem(int sectionId, int itemId)
        {
            var view = "/Views/Shared/Dashboard/About/_SectionItem.cshtml";
            var section = await _sectionService.GetByIdAsync(sectionId);
            var item = await _sectionService.GetItemByIdAsync(sectionId, itemId);

            ViewBag.Id = section.Id;
            ViewBag.Title = section.Title;
            ViewBag.Session = "Edit";
            ViewBag.Action = "EditSectionItem";

            return PartialView(view, item);
        }

        // POST: Edit Section Item
        [HttpPost]
        [Route("Sections/{sectionId}/{itemId}/Edit")]
        public async Task<IActionResult> EditSectionItem(int sectionId, AboutSectionItem item)
        {
            await _sectionService.UpdateItemAsync(sectionId, item);
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Section Item
        [HttpPost]
        [Route("Sections/{sectionId}/{itemId}/Delete")]
        public async Task<IActionResult> DeleteSectionItem(int sectionId, int itemId)
        {
            await _sectionService.DeleteItemAsync(sectionId, itemId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Reorder Section Items
        [HttpPost]
        [Route("Sections/{sectionId}/Reorder")]
        public async Task<IActionResult> ReorderSectionItems(AboutSection section)
        {
            await _sectionService.ReorderItemsAsync(section);
            return RedirectToAction(nameof(Index));
        }
    }
}
