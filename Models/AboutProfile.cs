namespace GhazwulShaf.Models;

public class AboutProfile
{
    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>()
    {
        {"Title", ""},
        {"Description", ""},
        {"Photo", ""}
    };
}