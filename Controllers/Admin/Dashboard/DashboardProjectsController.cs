using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin"), Route("/admin/dashboard/projects")]
    public class DashboardProjectsController : Controller
    {
        private readonly string _tempDir;
        private readonly string _deletedDir;
        private readonly ProjectsService _projectsService;
        private readonly MasterdataService _masterdataService;
        private readonly SlugHelper _slugHelper;

        public DashboardProjectsController(
            ProjectsService projectsService,
            MasterdataService masterdataService,
            SlugHelper slugHelper
        ) {
            _tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "_temp");

            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);

            _deletedDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "_archive", "deleted_projects");

            if (!Directory.Exists(_deletedDir))
                Directory.CreateDirectory(_deletedDir);

            _projectsService = projectsService;
            _masterdataService = masterdataService;
            _slugHelper = slugHelper;
        }

        // GET: Projects List Page
        [HttpGet, Route("")]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsService.GetAllAsync();
            var masterdata = await _masterdataService.GetAsync();
            
            int pageCurrent = 1;
            int pageSize = 10;
            int totalCount = projects.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            projects = projects.Take(pageSize).ToList();

            ViewBag.ProjectTypes = masterdata.Types;
            ViewBag.PagingInfo = new PagingInfo
            {
                CurrentPage = pageCurrent,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return View("/Views/Admin/Dashboard/Projects/Index.cshtml", projects);
        }

        // GET: Projects List with Search
        [HttpGet, Route("search")]
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

            int pageCurrent = 1;
            int pageSize = 10;
            int totalCount = projects.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            projects = projects.Take(pageSize).ToList();

            ViewBag.PagingInfo = new PagingInfo
            {
                CurrentPage = pageCurrent,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return PartialView("/Views/Admin/Dashboard/Projects/_Projects.cshtml", projects);
        }

        // GET: Projects Pagination
        [HttpGet, Route("page")]
        public async Task<IActionResult> SetPage(string projectType, string search, int page)
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

            int pageCurrent = page;
            int pageSize = 10;
            int totalCount = projects.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            projects = projects.Skip((pageCurrent - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.PagingInfo = new PagingInfo
            {
                CurrentPage = pageCurrent,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return PartialView("/Views/Admin/Dashboard/Projects/_Projects.cshtml", projects);
        }

        // GET: Add Project Page
        [HttpGet, Route("add")]
        public async Task<IActionResult> Add()
        {
            ViewData["Button"] = "Add";
            ViewData["FormAction"] = "Store";

            var masterdata = await _masterdataService.GetAsync();
            ViewBag.ProjectTypes = masterdata.Types;

            return View("/Views/Admin/Dashboard/Projects/Project.cshtml", new Project());
        }

        // GET: Add Gallery Image
        [HttpGet, Route("add/gallery/add")]
        public IActionResult AddImage(int imageNumber)
        {
            ViewBag.ImageNumber = imageNumber;

            return PartialView("/Views/Admin/Dashboard/Projects/_GalleryImage.cshtml");
        }

        // POST: Store All Gallery Images
        [HttpPost, Route("add/gallery/add/store")]
        public async Task<IActionResult> StoreImages(IFormFile[] files)
        {
            var uploadResults = new List<object>();

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var filePath = Path.Combine(_tempDir, Path.GetFileName(file.FileName));

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    
                    uploadResults.Add(new { fileName = file.FileName });
                }
            }

            return Ok(new { files = uploadResults });
        }

        // POST: Store New Project
        [HttpPost, Route("add/store")]
        public async Task<IActionResult> Store(Project project, IFormFile ThumbnailFile, string ProjectContent)
        {
            project.Guid = Guid.NewGuid();

            var slug = _slugHelper.GenerateSlug(project.Title ?? "");
            project.Slug = project.Guid.ToString()[..4] + $"-{slug}";

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

            var contentFileName = "content.html";
            var contentFilePath = Path.Combine(projectDir, contentFileName);

            if (System.IO.File.Exists(contentFilePath))
                System.IO.File.Delete(contentFilePath);
            
            await System.IO.File.WriteAllTextAsync(contentFilePath, ProjectContent);

            project.ContentFile = $"/projects/{project.Guid}/{contentFileName}";

            foreach (var file in project.GalleryFiles)
            {
                var filePath = Path.Combine(_tempDir, file);
                var destinationPath = Path.Combine(projectDir, file);

                System.IO.File.Move(filePath, destinationPath);
            }

            await _projectsService.AddAsync(project);

            return RedirectToAction("Index", "DashboardProjects");
        }

        // GET: Project Details and Edit Page
        [HttpGet, Route("{guid}/details")]
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
        [HttpPost, Route("{guid}/update")]
        public async Task<IActionResult> Update(Guid guid, Project project, IFormFile ThumbnailFile, string ProjectContent)
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

            foreach (var file in project.GalleryFiles)
            {
                var filePath = Path.Combine(_tempDir, file);
                var destinationPath = Path.Combine(projectDir, file);

                if (System.IO.File.Exists(destinationPath))
                    System.IO.File.Delete(destinationPath);

                System.IO.File.Move(filePath, destinationPath);
            }

            await _projectsService.UpdateAsync(project);

            return RedirectToAction("Index", "DashboardProjects");
        }

        // POST: Delete Project
        [HttpPost, Route("{guid}/delete")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var project = await _projectsService.GetByGuidAsync(guid);
            var projectDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", project.Guid.ToString());

            await _projectsService.DeleteAsync(guid);

            var destinationDir = Path.Combine(_deletedDir, project.Guid.ToString());
            Directory.Move(projectDir, destinationDir);
            
            return RedirectToAction("Index", "DashboardProjects");
        }

        // GET: Project Add Tag Partial
        [HttpGet, Route("tags/add")]
        public IActionResult AddTag(int itemId, string itemName)
        {
            ViewBag.ItemId = itemId;
            ViewBag.ItemName = itemName;

            return PartialView("/Views/Admin/Dashboard/Projects/_Tag.cshtml", new Project());
        }
    }
}
