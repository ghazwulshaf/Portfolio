namespace GhazwulShaf.Models;

public class About
{
    public AboutProfile Profile { get; set; } = new AboutProfile();
    public UploadFile ProfilePhoto { get; set; } = new UploadFile();
}