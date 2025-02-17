using GhazwulShaf.Models;
using Newtonsoft.Json;
using Slugify;

namespace GhazwulShaf.Services;

public class LearnService
{
    private readonly string _filePath;
    private readonly string _deletedPath;
    private readonly SlugHelper _slugHelper;

    public LearnService(SlugHelper slugHelper)
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "learn_modules.json");

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
            
        _deletedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "_deleted_learn_modules.json");

        if (!File.Exists(_deletedPath))
            File.WriteAllText(_deletedPath, "[]");
        
        _slugHelper = slugHelper;
    }

    public async Task<List<Learn>> GetAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        var learns = JsonConvert.DeserializeObject<List<Learn>>(json) ?? new List<Learn>();

        return learns;
    }

    public async Task<Learn> GetByIdAsync(int id)
    {
        var learns = await GetAllAsync();
        var learn = learns.FirstOrDefault(p => p.Id == id) ?? new Learn();

        return learn;
    }

    public async Task<Learn> GetByGuidAsync(Guid guid)
    {
        var learns = await GetAllAsync();
        var learn = learns.FirstOrDefault(p => p.Guid == guid) ?? new Learn();

        return learn;
    }

    public async Task<Learn> GetBySlugAsync(string slug)
    {
        var learns = await GetAllAsync();
        var learn = learns.FirstOrDefault(p => p.Slug == slug) ?? new Learn();

        return learn;
    }

    public async Task AddAsync(Learn learn)
    {
        var learns = await GetAllAsync();

        if (learn != null)
        {
            learn.Id = learns.Any() ? learns.Max(p => p.Id) + 1 : 0;
            learn.CreateDate = DateOnly.FromDateTime(DateTime.Now);
            learn.UpdateDate = DateOnly.FromDateTime(DateTime.Now);
            learns.Add(learn);
        }

        await WriteAsync(learns!);
    }

    public async Task UpdateAsync(Learn learn)
    {
        var learns = await GetAllAsync();
        var oldLearn = learns.FirstOrDefault(p => p.Guid == learn.Guid);

        if (oldLearn != null & learn != null)
        {
            learns[oldLearn!.Id].Title = learn!.Title;
            learns[oldLearn.Id].Type = learn.Type;
            learns[oldLearn.Id].Tags = learn.Tags;
            learns[oldLearn.Id].UpdateDate = DateOnly.FromDateTime(DateTime.Now);
        }

        await WriteAsync(learns);
    }

    public async Task DeleteAsync(int id)
    {
        var learns = await GetAllAsync();
        var learn = learns.FirstOrDefault(p => p.Id == id);

        if (learn != null)
        {
            learns.Remove(learn);
        }

        await WriteAsync(learns);
    
    }
    
    public async Task DeleteAsync(Guid guid)
    {
        var learns = await GetAllAsync();
        var learn = learns.FirstOrDefault(p => p.Guid == guid);

        var deletedJson = await File.ReadAllTextAsync(_deletedPath);
        var deletedLearn = JsonConvert.DeserializeObject<List<Learn>>(deletedJson) ?? new List<Learn>();

        if (learn != null)
        {
            learns.Remove(learn);

            learn.DeleteDate = DateOnly.FromDateTime(DateTime.Now);
            deletedLearn.Add(learn);
        }

        await WriteAsync(learns);

        var deletedWriteJson = JsonConvert.SerializeObject(deletedLearn, Formatting.Indented);
        await File.WriteAllTextAsync(_deletedPath, deletedWriteJson);
    }

    public async Task WriteAsync(List<Learn> learns)
    {
        await Task.Run(() =>
        {
            if (learns.Count != 0)
            {
                for (int i = 0; i < learns.Count; i++)
                {
                    learns[i].Id = i;
                    var tagsDontUse = new List<Tag>();

                    foreach (var tag in learns[i].Tags)
                    {
                        if (tag.Name == null)
                            tagsDontUse.Add(tag);
                    }

                    foreach (var tag in tagsDontUse)
                        learns[i].Tags.Remove(tag);

                    for (int j = 0; j < learns[i].Tags.Count; j++)
                    {
                        learns[i].Tags[j].Id = j;
                    }
                }
            }
        });

        learns = await UpdateSlug(learns);

        var json = JsonConvert.SerializeObject(learns, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private async Task<List<Learn>> UpdateSlug(List<Learn> learns)
    {
        await Task.Run(() =>
        {
            foreach (var learn in learns)
            {
                var slug = _slugHelper.GenerateSlug(learn.Title ?? "");
                learn.Slug = learn.Guid.ToString()[..4] + $"-{slug}";
            }
        });

        return learns;
    }
}