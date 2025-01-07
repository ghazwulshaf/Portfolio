using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class AboutProfileService
{
    private readonly string _filePath;

    public AboutProfileService(IConfiguration configuration)
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "about_profile.json");

        if (!File.Exists(_filePath))
        {
            var profile = new AboutProfile();
            var json = JsonConvert.SerializeObject(profile, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }

    public async Task<AboutProfile> GetAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        var profile = JsonConvert.DeserializeObject<AboutProfile>(json) ?? new AboutProfile();
        return profile;
    }

    public async Task UpdateAsync(AboutProfile profile)
    {
        var json = JsonConvert.SerializeObject(profile, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}