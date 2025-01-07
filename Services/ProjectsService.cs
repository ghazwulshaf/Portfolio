using GhazwulShaf.Models;
using Newtonsoft.Json;
using Slugify;

namespace GhazwulShaf.Services;

public class ProjectsService
{
    private readonly string _filePath;
    private readonly string _deletedPath;
    private readonly SlugHelper _slugHelper;

    public ProjectsService(SlugHelper slugHelper)
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "projects.json");

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");

        _deletedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "_deleted_projects.json");

        if (!File.Exists(_deletedPath))
            File.WriteAllText(_deletedPath, "[]");
        
        _slugHelper = slugHelper;
    }

    public async Task<List<Project>> GetAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        var projects = JsonConvert.DeserializeObject<List<Project>>(json) ?? new List<Project>();

        return projects;
    }

    public async Task<Project> GetByIdAsync(int id)
    {
        var projects = await GetAllAsync();
        var project = projects.FirstOrDefault(p => p.Id == id) ?? new Project();

        return project;
    }

    public async Task<Project> GetByGuidAsync(Guid guid)
    {
        var projects = await GetAllAsync();
        var project = projects.FirstOrDefault(p => p.Guid == guid) ?? new Project();

        return project;
    }

    public async Task<Project> GetBySlugAsync(string slug)
    {
        var projects = await GetAllAsync();
        var project = projects.FirstOrDefault(p => p.Slug == slug) ?? new Project();

        return project;
    }

    public async Task AddAsync(Project project)
    {
        var projects = await GetAllAsync();

        if (project != null)
        {
            project.Id = projects.Any() ? projects.Max(p => p.Id) + 1 : 0;
            project.CreateDate = DateOnly.FromDateTime(DateTime.Now);
            project.UpdateDate = DateOnly.FromDateTime(DateTime.Now);
            projects.Add(project);
        }

        await WriteAsync(projects!);
    }

    public async Task UpdateAsync(Project project)
    {
        var projects = await GetAllAsync();
        var oldProject = projects.FirstOrDefault(p => p.Guid == project.Guid);

        if (oldProject != null & project != null)
        {
            projects[oldProject!.Id].Title = project!.Title;
            projects[oldProject.Id].Type = project.Type;
            projects[oldProject.Id].Tags = project.Tags;
            projects[oldProject.Id].LiveView = project.LiveView;
            projects[oldProject.Id].CodeView = project.CodeView;
            projects[oldProject.Id].GalleryFiles = project.GalleryFiles;
            projects[oldProject.Id].UpdateDate = DateOnly.FromDateTime(DateTime.Now);
        }

        await WriteAsync(projects);
    }

    public async Task DeleteAsync(Guid guid)
    {
        var projects = await GetAllAsync();
        var project = projects.FirstOrDefault(p => p.Guid == guid);

        var deletedJson = await File.ReadAllTextAsync(_deletedPath);
        var deletedProjects = JsonConvert.DeserializeObject<List<Project>>(deletedJson) ?? new List<Project>();

        if (project != null)
        {
            projects.Remove(project);

            project.DeleteDate = DateOnly.FromDateTime(DateTime.Now);
            deletedProjects.Add(project);
        }

        await WriteAsync(projects);

        var deletedWriteJson = JsonConvert.SerializeObject(deletedProjects, Formatting.Indented);
        await File.WriteAllTextAsync(_deletedPath, deletedWriteJson);
    }

    private async Task WriteAsync(List<Project> projects)
    {
        await Task.Run(() =>
        {
            if (projects.Count != 0)
            {
                for (int i = 0; i < projects.Count; i++)
                {
                    projects[i].Id = i;
                    var tagsDontUse = new List<Tag>();

                    foreach (var tag in projects[i].Tags)
                    {
                        if (tag.Name == null)
                            tagsDontUse.Add(tag);
                    }

                    foreach (var tag in tagsDontUse)
                        projects[i].Tags.Remove(tag);

                    for (int j = 0; j < projects[i].Tags.Count; j++)
                    {
                        projects[i].Tags[j].Id = j;
                    }
                }
            }
        });

        var json = JsonConvert.SerializeObject(projects, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private async Task<List<Project>> UpdateSlug(List<Project> projects)
    {
        await Task.Run(() =>
        {
            foreach (var project in projects)
            {
                var slug = _slugHelper.GenerateSlug(project.Title ?? "");
                project.Slug = project.Guid.ToString()[..4] + $"-{slug}";
            }
        });

        return projects;
    }
}