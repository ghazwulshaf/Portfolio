using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class ProjectsService
{
    private readonly string _filePath;

    public ProjectsService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "projects.json");

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
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
        var oldProject = projects.FirstOrDefault(p => p.Id == project.Id);

        if (oldProject != null & project != null)
        {
            project!.CreateDate = oldProject!.CreateDate;
            project!.UpdateDate = DateOnly.FromDateTime(DateTime.Now);
            projects[oldProject!.Id] = project!;
        }

        await WriteAsync(projects);
    }

    public async Task DeleteAsync(int id)
    {
        var projects = await GetAllAsync();
        var project = projects.FirstOrDefault(p => p.Id == id);

        if (project != null)
        {
            projects.Remove(project);
        }

        await WriteAsync(projects);
    }

    public async Task WriteAsync(List<Project> projects)
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
}