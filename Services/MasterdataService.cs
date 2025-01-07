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

    public async Task UpdateAsync(Masterdata masterdata)
    {
        // delete prject types item don't use
        var typesDontUse = new List<ProjectType>();

        // get items don't use
        foreach (var item in masterdata.Types)
        {
            if (item.Name == null)
                typesDontUse.Add(item);
        }

        // delete items don't use
        foreach (var item in typesDontUse)
            masterdata.Types.Remove(item);
        
        // optimize Id
        for (int i = 0; i < masterdata.Types.Count; i++)
            masterdata.Types[i].Id = i;
        
        // update file
        var json = JsonConvert.SerializeObject(masterdata, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}