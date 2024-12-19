using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Route("/Admin/Dashboard/Projects")]
    public class DashboardProjectsController : Controller
    {
        private readonly ProjectsService _projectsService;
        private readonly MasterdataService _masterdataService;

        public DashboardProjectsController(ProjectsService projectsService, MasterdataService masterdataService)
        {
            _projectsService = projectsService;
            _masterdataService = masterdataService;
        }

        // GET: Projects List Page
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsService.GetAllAsync();
            var masterdata = await _masterdataService.GetAsync();

            ViewBag.ProjectTypes = masterdata.Types;

            return View("/Views/Admin/Dashboard/Projects/Index.cshtml", projects);
        }

        // GET: Projects List with Search
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search(string projectType, string search)
        {
            var projects = await _projectsService.GetAllAsync();

            if (projectType != "All")
            {
                projects = projects.Where(p => p.Type == projectType).ToList();
            }

            if (!String.IsNullOrEmpty(search))
            {
                projects = projects.Where(p =>
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Tags.Any(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            return PartialView("/Views/Admin/Dashboard/Projects/_Projects.cshtml", projects);
        }

        // GET: Add Project Page
        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            ViewData["Button"] = "Add";
            ViewData["FormAction"] = "Store";

            var masterdata = await _masterdataService.GetAsync();
            ViewBag.ProjectTypes = masterdata.Types;

            return View("/Views/Admin/Dashboard/Projects/Project.cshtml", new Project());
        }

        // POST: Store New Project
        [HttpPost]
        [Route("Add/Store")]
        public async Task<IActionResult> Store(Project project, IFormFile ThumbnailFile, String ProjectContent)
        {
            project.Guid = Guid.NewGuid();

            var projectDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", project.Guid.ToString());

            if (!Directory.Exists(projectDir))
                Directory.CreateDirectory(projectDir);

            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
            {
                var thumbnailFileName = "thumbnail" + Path.GetExtension(ThumbnailFile.FileName);
                var thumbnailFilePath = Path.Combine(projectDir, thumbnailFileName);

                if (System.IO.File.Exists(thumbnailFilePath))
                    System.IO.File.Delete(thumbnailFilePath);

                using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
                {
                    await ThumbnailFile.CopyToAsync(stream);
                }

                project.Thumbnail = $"/projects/{project.Guid}/{thumbnailFileName}";
            }

            if (ProjectContent != null)
            {
                var contentFileName = "content.html";
                var contentFilePath = Path.Combine(projectDir, contentFileName);

                if (System.IO.File.Exists(contentFilePath))
                    System.IO.File.Delete(contentFilePath);
                
                await System.IO.File.WriteAllTextAsync(contentFilePath, ProjectContent);

                project.ContentFile = $"/projects/{project.Guid}/{contentFileName}";
            }

            await _projectsService.AddAsync(project);

            return RedirectToAction("Index", "DashboardProjects");
        }

        // GET: Project Details and Edit Page
        [HttpGet]
        [Route("{guid}/Details")]
        public async Task<IActionResult> Details(Guid guid)
        {
            var project = await _projectsService.GetByGuidAsync(guid);

            ViewData["Button"] = "Save";
            ViewData["FormAction"] = "Update";

            var masterdata = await _masterdataService.GetAsync();
            ViewBag.ProjectTypes = masterdata.Types;

            return View("/Views/Admin/Dashboard/Projects/Project.cshtml", project);
        }

        // POST: Update Project
        [HttpPost]
        [Route("{guid}/Update")]
        public async Task<IActionResult> Update(Guid guid, Project project, IFormFile ThumbnailFile, String ProjectContent)
        {
            var projectDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", project.Guid.ToString());

            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
            {
                var thumbnailFileName = "thumbnail" + Path.GetExtension(ThumbnailFile.FileName);
                var thumbnailFilePath = Path.Combine(projectDir, thumbnailFileName);

                if (System.IO.File.Exists(thumbnailFilePath))
                    System.IO.File.Delete(thumbnailFilePath);

                using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
                {
                    await ThumbnailFile.CopyToAsync(stream);
                }

                project.Thumbnail = $"/projects/{project.Guid}/{thumbnailFileName}";
            }

            var contentFileName = "content.html";
            var contentFilePath = Path.Combine(projectDir, contentFileName);
            await System.IO.File.WriteAllTextAsync(contentFilePath, ProjectContent);

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
