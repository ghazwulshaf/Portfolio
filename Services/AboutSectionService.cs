using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class AboutSectionService
{
    private readonly string _filePath;

    public AboutSectionService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "about_sections.json");

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<List<AboutSection>> GetAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        var sections = JsonConvert.DeserializeObject<List<AboutSection>>(json) ?? new List<AboutSection>();

        await OptimizeId(sections);
        
        return JsonConvert.DeserializeObject<List<AboutSection>>(json) ?? new List<AboutSection>();
    }

    public async Task<AboutSection> GetByIdAsync(int id)
    {
        var sections = await GetAllAsync();
        var section = sections.FirstOrDefault(s => s.Id == id) ?? new AboutSection();
        
        return section;
    }

    public async Task<AboutSectionItem> GetItemByIdAsync(int id, int itemId)
    {
        var sections = await GetAllAsync();
        var section = sections.FirstOrDefault(s => s.Id == id) ?? new AboutSection();
        var item = section.Items.FirstOrDefault(i => i.Id == itemId) ?? new AboutSectionItem();

        return item;
    }

    public async Task AddAsync(AboutSection section)
    {
        var sections = await GetAllAsync();

        section.Id = sections.Any() ? sections.Max(s => s.Id) + 1 : 0;
        sections.Add(section);

        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task AddItemAsync(int id, AboutSectionItem item)
    {
        var sections = await GetAllAsync();
        var section = sections.FirstOrDefault(s => s.Id == id);

        if (section != null)
        {
            item.Id = section.Items.Any() ? section.Items.Max(i => i.Id) + 1 : 0;
            section.Items.Add(item);
            sections[id] = section;
        }

        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task UpdateAsync(AboutSection section)
    {
        var sections = await GetAllAsync();
        var oldSection = sections.FirstOrDefault(s => s.Id == section.Id);
        
        if (oldSection != null)
            sections[section.Id].Id = section.Id;
            sections[section.Id].Title = section.Title;
        
        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task UpdateItemAsync(int id, AboutSectionItem item)
    {
        var sections = await GetAllAsync();
        var section = sections.FirstOrDefault(s => s.Id == id);

        if (section != null)
        {
            var oldItem = section.Items.FirstOrDefault(i => i.Id == item.Id);

            if (oldItem != null)
                section.Items[item.Id] = item;
        }

        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task DeleteAsync(int id)
    {
        var sections = await GetAllAsync();
        var section = sections.FirstOrDefault(s => s.Id == id);

        if (section != null)
            sections.Remove(section);
        
        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task DeleteItemAsync(int id, int itemId)
    {
        var sections = await GetAllAsync();
        var section = sections.FirstOrDefault(s => s.Id == id);

        if (section != null)
        {
            var item = section.Items.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
                section.Items.Remove(item);
        }

        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task ReorderItemsAsync(AboutSection section)
    {
        var sections = await GetAllAsync();
        var oldSection = sections.FirstOrDefault(s => s.Id == section.Id);

        if (oldSection != null)
        {
            for (int i = 0; i < oldSection.Items.Count; i++)
                oldSection.Items[i].Order = section.Items[i].Order;

            oldSection.Items = oldSection.Items.OrderBy(i => i.Order).ToList();
        }
        
        await OptimizeId(sections);
        await WriteAsync(sections);
    }

    public async Task OptimizeId(List<AboutSection> sections)
    {
        await Task.Run(() =>
        {
            if (sections.Count != 0)
                for (var i = 0; i < sections.Count; i++)
                {
                    sections[i].Id = i;

                    if (sections[i].Items.Count != 0)
                        for (var j = 0; j < sections[i].Items.Count; j++)
                        {
                            sections[i].Items[j].Id = j;
                            sections[i].Items[j].Order = j;
                        }
                }
        });
    }

    public async Task WriteAsync(List<AboutSection> sections)
    {
        var json = JsonConvert.SerializeObject(sections, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}