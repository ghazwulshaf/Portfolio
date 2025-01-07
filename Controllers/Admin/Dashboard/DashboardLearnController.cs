using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace GhazwulShaf.Controllers.Admin.Dashboard;

[Authorize(Roles = "Admin"), Route("/admin/dashboard/learn")]
public class DashboardLearnController : Controller
{
    private readonly string _deletedDir;
    private readonly LearnService _learnService;
    private readonly MasterdataService _masterdataService;
    private readonly SlugHelper _slugHelper;

    private readonly int _pageSize = 10;

    public DashboardLearnController(
        LearnService learnService,
        MasterdataService masterdataService,
        SlugHelper slugHelper
    ) {
        _deletedDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "_archive", "deleted_learn_modules");

        if (!Directory.Exists(_deletedDir))
            Directory.CreateDirectory(_deletedDir);
        
        _learnService = learnService;
        _masterdataService = masterdataService;
        _slugHelper = slugHelper;
    }

    // GET: Dashboard Learn Modules List Page
    [HttpGet, Route("")]
    public async Task<IActionResult> Index()
    {
        var learns = await _learnService.GetAllAsync();
        var masterdata = await _masterdataService.GetAsync();
            
        int pageCurrent = 1;
        int pageSize = _pageSize;
        int totalCount = learns.Count;
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        learns = learns.Take(pageSize).ToList();

        ViewBag.ProjectTypes = masterdata.Types;
        ViewBag.PagingInfo = new PagingInfo
        {
            CurrentPage = pageCurrent,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return View("/Views/Admin/Dashboard/Learn/Index.cshtml", learns);
    }

    // GET: Show Learn Modules List by Search
    [HttpGet, Route("search")]
    public async Task<IActionResult> Search(string projectType, string search)
    {
        var learns = await _learnService.GetAllAsync();

        if (projectType != "All")
        {
            learns = learns.Where(p => p.Type == projectType).ToList();
        }

        if (!String.IsNullOrEmpty(search))
        {
            learns = learns.Where(p =>
                p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Tags.Any(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        int pageCurrent = 1;
        int pageSize = _pageSize;
        int totalCount = learns.Count;
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        learns = learns.Take(pageSize).ToList();

        ViewBag.PagingInfo = new PagingInfo
        {
            CurrentPage = pageCurrent,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return PartialView("/Views/Admin/Dashboard/Learn/_Modules.cshtml", learns);
    }

    // GET: Show Learn Modules List by Page
    [HttpGet, Route("pages")]
    public async Task<IActionResult> SetPage(string projectType, string search, int page)
    {
        var learns = await _learnService.GetAllAsync();

        if (projectType != "All")
        {
            learns = learns.Where(p => p.Type == projectType).ToList();
        }

        if (!String.IsNullOrEmpty(search))
        {
            learns = learns.Where(p =>
                p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Tags.Any(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        int pageCurrent = page;
        int pageSize = _pageSize;
        int totalCount = learns.Count;
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        learns = learns.Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.PagingInfo = new PagingInfo
        {
            CurrentPage = pageCurrent,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return PartialView("/Views/Admin/Dashboard/Learn/_Projects.cshtml", learns);
    }

    // GET: Dashboard Add Learn Module Page
    [HttpGet, Route("add")]
    public async Task<IActionResult> Add()
    {
        ViewData["Button"] = "Add";
        ViewData["FormAction"] = "Store";

        var masterdata = await _masterdataService.GetAsync();
        ViewBag.ProjectTypes = masterdata.Types;

        return View("/Views/Admin/Dashboard/Learn/Module.cshtml", new Learn());
    }

    // GET: Dashboard Learn Module Add Tag Partial
    [HttpGet, Route("tags/add")]
    public IActionResult AddTag(int itemId, string itemName)
    {
        ViewBag.ItemId = itemId;
        ViewBag.ItemName = itemName;

        return PartialView("/Views/Admin/Dashboard/Learn/_Tag.cshtml", new Learn());
    }

    // POST: Store New Learn Module
    [HttpPost, Route("add/store")]
    public async Task<IActionResult> Store(Learn learn, IFormFile thumbnailFile, string learnContent)
    {
        learn.Guid = Guid.NewGuid();

        var slug = _slugHelper.GenerateSlug(learn.Title ?? "");
        learn.Slug = learn.Guid.ToString()[..4] + $"-{slug}";

        var learnDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "learn_modules", learn.Guid.ToString());

        if (!Directory.Exists(learnDir))
            Directory.CreateDirectory(learnDir);

        if (thumbnailFile != null && thumbnailFile.Length > 0)
        {
            var thumbnailFileName = "thumbnail" + Path.GetExtension(thumbnailFile.FileName);
            var thumbnailFilePath = Path.Combine(learnDir, thumbnailFileName);

            if (System.IO.File.Exists(thumbnailFilePath))
                System.IO.File.Delete(thumbnailFilePath);

            using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
            {
                await thumbnailFile.CopyToAsync(stream);
            }

            learn.Thumbnail = $"/learn_modules/{learn.Guid}/{thumbnailFileName}";
        }

        var contentFileName = "content.html";
        var contentFilePath = Path.Combine(learnDir, contentFileName);

        if (System.IO.File.Exists(contentFilePath))
            System.IO.File.Delete(contentFilePath);
        
        await System.IO.File.WriteAllTextAsync(contentFilePath, learnContent);

        learn.ContentFile = $"/learn_modules/{learn.Guid}/{contentFileName}";

        await _learnService.AddAsync(learn);

        return RedirectToAction("Index", "DashboardLearn");
    }

    // GET: Learn Module Details Page
    [HttpGet, Route("{guid}/details")]
    public async Task<IActionResult> Details(Guid guid)
    {
        var learn = await _learnService.GetByGuidAsync(guid);

        ViewData["Button"] = "Save";
        ViewData["FormAction"] = "Update";

        var masterdata = await _masterdataService.GetAsync();
        ViewBag.ProjectTypes = masterdata.Types;

        return View("/Views/Admin/Dashboard/Learn/Module.cshtml", learn);
    }

    // POST: Update Learn Module Data
    [HttpPost, Route("{guid}/update")]
    public async Task<IActionResult> Update(Guid guid, Learn learn, IFormFile thumbnailFile, string learnContent)
    {
        var learnDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "learn_modules", learn.Guid.ToString());

        if (thumbnailFile != null && thumbnailFile.Length > 0)
        {
            var thumbnailFileName = "thumbnail" + Path.GetExtension(thumbnailFile.FileName);
            var thumbnailFilePath = Path.Combine(learnDir, thumbnailFileName);

            if (System.IO.File.Exists(thumbnailFilePath))
                System.IO.File.Delete(thumbnailFilePath);

            using (var stream = new FileStream(thumbnailFilePath, FileMode.Create))
            {
                await thumbnailFile.CopyToAsync(stream);
            }

            learn.Thumbnail = $"/learn_modules/{learn.Guid}/{thumbnailFileName}";
        }

        var contentFileName = "content.html";
        var contentFilePath = Path.Combine(learnDir, contentFileName);

        await System.IO.File.WriteAllTextAsync(contentFilePath, learnContent);

        await _learnService.UpdateAsync(learn);

        return RedirectToAction("Index", "DashboardLearn");
    }

    // POST: Delete Learn Modules Item
    [HttpPost, Route("{guid}/delete")]
    public async Task<IActionResult> Delete(Guid guid)
    {
        var learn = await _learnService.GetByGuidAsync(guid);
        var learnDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "learn_modules", learn.Guid.ToString());
        
        await _learnService.DeleteAsync(guid);

        var destinationDir = Path.Combine(_deletedDir, learn.Guid.ToString());
        Directory.Move(learnDir, destinationDir);

        return RedirectToAction("Index", "DashboardLearn");
    }
}