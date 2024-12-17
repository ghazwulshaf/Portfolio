using System.ComponentModel.DataAnnotations;

namespace GhazwulShaf.Models;

public class Project
{
    public int Id { get; set;}
    
    [Required]
    public string Title { get; set;} = default!;
    
    public string Thumbnail { get; set; } = default!;
    public DateOnly CreateDate { get; set; } = new DateOnly();
    public DateOnly UpdateDate { get; set;} = new DateOnly();
    public string Type { get; set;} = default!;
    public List<Tag> Tags { get; set; } = [];
    public string Contents { get; set;} = default!;
}