using System.ComponentModel.DataAnnotations;

namespace GhazwulShaf.Models;

public class Project
{
    public int Id { get; set;}
    public string Title { get; set;} = default!;
    public string Thumbnail { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string Type { get; set;} = default!;
    public List<Tag> Tags { get; set; } = [];
    public string Contents { get; set;} = default!;
}