using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class LearnService
{
    private readonly string _filePath;

    public LearnService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "learn_modules.json");

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
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

        if (learn != null)
        {
            learns.Remove(learn);
        }

        await WriteAsync(learns);
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

        var json = JsonConvert.SerializeObject(learns, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}