using System.ComponentModel.DataAnnotations;

namespace GhazwulShaf.Models;

public class Learn
{
    public int Id { get; set;}
    public Guid Guid { get; set;}
    public string Slug { get; set;} = default!;
    
    [Required]
    public string Title { get; set;} = default!;
    
    public string Description { get; set;} = default!;
    public string Thumbnail { get; set; } = default!;
    public DateOnly CreateDate { get; set; } = new DateOnly();
    public DateOnly UpdateDate { get; set;} = new DateOnly();
    public DateOnly DeleteDate { get; set;} = new DateOnly();
    public string Type { get; set;} = default!;
    public List<Tag> Tags { get; set; } = [];
    
    public string ContentFile { get; set;} = default!;
}