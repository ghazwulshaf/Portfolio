namespace GhazwulShaf.Models;

public class About
{
    public AboutProfile Profile { get; set; } = new AboutProfile();
    public List<AboutSection> Sections { get; set; } = [];
}