using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home;

[Route("/learn")]
public class LearnController : Controller
{
    private readonly LearnService _learnService;
    private readonly MasterdataService _masterdataService;

    private readonly int _pageSize = 12;

    public LearnController(LearnService learnService, MasterdataService masterdataService)
    {
        _learnService = learnService;
        _masterdataService = masterdataService;
    }

    // GET: Learn Page
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

        return View("/Views/Home/Learn/Index.cshtml", learns);
    }

    // GET: Show Learn Modules List by Filter
    [HttpGet, Route("filter")]
    public async Task<IActionResult> Filter(string projectType, string search)
    {
        var learns = await _learnService.GetAllAsync();

        if (projectType != "All")
        {
            learns = learns.Where(p => p.Type == projectType).ToList();
        }

        if (!string.IsNullOrEmpty(search))
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

        return PartialView("/Views/Home/Learn/_Modules.cshtml", learns);
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

        if (!string.IsNullOrEmpty(search))
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

        return PartialView("/Views/Home/Learn/_Modules.cshtml", learns);
    }

    // GET: Learn Module Details
    [HttpGet, Route("{slug}/details")]
    public async Task<IActionResult> Details(string slug)
    {
        var learns = await _learnService.GetAllAsync();
        var learn = await _learnService.GetBySlugAsync(slug);
        var nextLearn = learns.FirstOrDefault(p => p.Id == learn.Id + 1) ?? null;

        var learnsSameType = learns.Where(p => p.Type == learn.Type)
            .OrderByDescending(p => p.Id)
            .Take(5)
            .ToList();
        
        ViewBag.NextLearn = nextLearn;
        ViewBag.LearnsSameType = learnsSameType;

        return View("/Views/Home/Learn/Details.cshtml", learn);
    }
}