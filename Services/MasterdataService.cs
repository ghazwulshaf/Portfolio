using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class MasterdataService
{
    private readonly string _filePath;

    public MasterdataService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "masterdata.json");

        if (!File.Exists(_filePath))
        {
            var masterdata = new Masterdata();
            var json = JsonConvert.SerializeObject(masterdata, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }

    public async Task<Masterdata> GetAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        var masterdata = JsonConvert.DeserializeObject<Masterdata>(json) ?? new Masterdata();

        return masterdata;
    }

    public async Task UpdateWelcomeAsync(Welcome welcome)
    {
        var masterdata = await GetAsync();
        masterdata.Welcome = welcome;

        await WriteAsync(masterdata);
    }

    public async Task UpdateTypesAsync(List<ProjectType> types)
    {
        var masterdata = await GetAsync();

        var typesDontUse = new List<ProjectType>();
        foreach (var item in types)
        {
            if (item.Name == null)
                typesDontUse.Add(item);
        }

        foreach (var item in typesDontUse)
            types.Remove(item);
        
        for (int i = 0; i < masterdata.Types.Count; i++)
            types[i].Id = i;
        
        masterdata.Types = types;
        
        await WriteAsync(masterdata);
    }

    private async Task WriteAsync(Masterdata masterdata)
    {
        var json = JsonConvert.SerializeObject(masterdata, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}