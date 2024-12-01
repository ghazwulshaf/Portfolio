using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class AboutProfileService
{
    private readonly string _filePath;

    public AboutProfileService(IConfiguration configuration)
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "about_profile.json");
    }

    public async Task<AboutProfile> GetAsync()
    {
        AboutProfile profile = new AboutProfile();
        Dictionary<string, string> profileData = new Dictionary<string, string>();

        if (!System.IO.File.Exists(_filePath))
            await UpdateAsync(profile);
        else
        {
            var data = await System.IO.File.ReadAllTextAsync(_filePath);
            profileData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data)!;
            profile.Data = profileData;
        }

        return profile;
    }

    public async Task UpdateAsync(AboutProfile profile)
    {
        if (profile.Data == null)
        {
            AboutProfile initProfile = new AboutProfile();
            profile.Data = initProfile.Data;
        }

        var profileJson = JsonConvert.SerializeObject(profile.Data, Formatting.Indented);
        await System.IO.File.WriteAllTextAsync(_filePath, profileJson);
    }
}