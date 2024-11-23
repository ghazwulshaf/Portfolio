namespace GhazwulShaf.Models;

public class About
{
    public Profile Profile { get; set; } = new Profile();
    public UploadFile ProfilePhoto { get; set; } = new UploadFile();
}