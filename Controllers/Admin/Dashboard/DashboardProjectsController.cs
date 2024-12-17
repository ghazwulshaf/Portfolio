using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Route("/Admin/Dashboard/Projects")]
    public class DashboardProjectsController : Controller
    {
        private readonly ProjectsService _projectsService;

        public DashboardProjectsController(ProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        // GET: Projects List Page
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsService.GetAllAsync();
            return View("/Views/Admin/Dashboard/Projects/Index.cshtml", projects);
        }

        // GET: Project Details and Edit Page
        [HttpGet]
        [Route("{id}/Details")]
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectsService.GetByIdAsync(id);

            ViewData["Button"] = "Save";
            ViewData["FormAction"] = "Update";

            return View("/Views/Admin/Dashboard/Projects/Project.cshtml", project);
        }

        // GET: Add Project Page
        [HttpGet]
        [Route("Add")]
        public IActionResult Add()
        {
            ViewData["Button"] = "Add";
            ViewData["FormAction"] = "Store";

            return View("/Views/Admin/Dashboard/Projects/Project.cshtml", new Project());
        }

        // POST: Store New Project
        [HttpPost]
        [Route("Add/Store")]
        public async Task<IActionResult> Store(Project project, IFormFile thumbnailFile)
        {
            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var projectTitle = project.Title.Humanize().Underscore().ToLower();
                
                var thumbnailFileName = Path.GetFileName(thumbnailFile.FileName);
                var thumbnailFileDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "projects", projectTitle);
                var thumbnailFilePath = Path.Combine(thumbnailFileDir, thumbnailFileName);

                if (!Directory.Exists(thumbnailFileDir))
                    Directory.CreateDirectory(thumbnailFileDir);

                if (System.IO.File.Exists(thumbnailFilePath))
                    System.IO.File.Delete(thumbnailFilePath);

                using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
                {
                    await thumbnailFile.CopyToAsync(stream);
                }

                project.Thumbnail = "/images/projects/" + projectTitle + "/" + thumbnailFileName;
            }
            await _projectsService.AddAsync(project);

            return RedirectToAction("Index", "DashboardProjects");
        }

        // POST: Update Project
        [HttpPost]
        [Route("{id}/Update")]
        public async Task<IActionResult> Update(Project project, IFormFile thumbnailFile)
        {
            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var projectTitle = project.Title.Humanize().Underscore().ToLower();
                
                var thumbnailFileName = Path.GetFileName(thumbnailFile.FileName);
                var thumbnailFileDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "projects", projectTitle);
                var thumbnailFilePath = Path.Combine(thumbnailFileDir, thumbnailFileName);

                if (!Directory.Exists(thumbnailFileDir))
                    Directory.CreateDirectory(thumbnailFileDir);

                if (System.IO.File.Exists(thumbnailFilePath))
                    System.IO.File.Delete(thumbnailFilePath);

                using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
                {
                    await thumbnailFile.CopyToAsync(stream);
                }

                project.Thumbnail = "/images/projects/" + projectTitle + "/" + thumbnailFileName;
            }
            await _projectsService.UpdateAsync(project);

            return RedirectToAction("Index", "DashboardProjects");
        }

        // POST: Delete Project
        [HttpPost]
        [Route("{id}/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectsService.DeleteAsync(id);
            return RedirectToAction("Index", "DashboardProjects");
        }

        // GET: Project Add Tag Partial
        [HttpGet]
        [Route("Tags/Add")]
        public IActionResult AddTag(int itemId, string itemName)
        {
            ViewBag.ItemId = itemId;
            ViewBag.ItemName = itemName;

            return PartialView("/Views/Admin/Dashboard/Projects/_Tag.cshtml", new Project());
        }
    }
}
