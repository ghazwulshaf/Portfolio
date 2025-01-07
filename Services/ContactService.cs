using GhazwulShaf.Models;
using Newtonsoft.Json;

namespace GhazwulShaf.Services;

public class ContactService
{
    private readonly string _filePath;

    public ContactService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "contact.json");

        if (!File.Exists(_filePath))
        {
            var contact = new Contact();
            var json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }

    public async Task<Contact> GetAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        var contact = JsonConvert.DeserializeObject<Contact>(json) ?? new Contact();

        return contact;
    }

    public async Task UpdateAsync(Contact contact)
    {
        contact ??= new Contact();
        var json = JsonConvert.SerializeObject(contact, Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}