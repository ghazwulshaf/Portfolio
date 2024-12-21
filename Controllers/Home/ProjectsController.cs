using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home
{
    [Route("/projects")]
    public class ProjectsController : Controller
    {
        private readonly ProjectsService _projectsService;
        private readonly MasterdataService _masterdataService;

        public ProjectsController(ProjectsService projectsService, MasterdataService masterdataService)
        {
            _projectsService = projectsService;
            _masterdataService = masterdataService;
        }

        // GET: Projects List
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsService.GetAllAsync();
            var masterdata = await _masterdataService.GetAsync();

            int pageCurrent = 1;
            int pageSize = 6;
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

            return View("/Views/Home/Projects/Index.cshtml", projects);
        }

        // POST: Projects List Filter
        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> Filter(string projectType, string search)
        {
            var projects = await _projectsService.GetAllAsync();

            if (projectType != "All")
            {
                projects = projects.Where(p => p.Type == projectType).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                projects = projects.Where(p =>
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Tags.Any(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            int pageCurrent = 1;
            int pageSize = 6;
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

            return PartialView("/Views/Home/Projects/_Projects.cshtml", projects);
        }

        // GET: Set Page
        [HttpGet]
        [Route("pages")]
        public async Task<IActionResult> SetPage(string projectType, string search, int page)
        {
            var projects = await _projectsService.GetAllAsync();

            if (projectType != "All")
            {
                projects = projects.Where(p => p.Type == projectType).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                projects = projects.Where(p =>
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Tags.Any(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            int pageCurrent = page;
            int pageSize = 6;
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

            return PartialView("/Views/Home/Projects/_Projects.cshtml", projects);
        }

        // GET: Project Details
        [HttpGet]
        [Route("{id}/Details")]
        public async Task<IActionResult> Details(int id)
        {
            var projects = await _projectsService.GetAllAsync();
            var project = await _projectsService.GetByIdAsync(id);
            var nextProject = projects.FirstOrDefault(p => p.Id == id + 1) ?? null;

            var projectsSameType = projects.Where(p => p.Type == project.Type)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToList();
            
            ViewBag.NextProject = nextProject;
            ViewBag.ProjectsSameType = projectsSameType;

            return View("/Views/Home/Projects/Details.cshtml", project);
        }
    }
}
