using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace GhazwulShaf.Models;

public class AboutSectionItem
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Company { get; set; }
    public string? Description { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? From { get; set; }

    [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? To { get; set; }
}

public class AboutSection
{
    public int Id { get; set; } = default!;
    
    [Required]
    public string Title { get; set; } = default!;
    
    public List<AboutSectionItem> Items { get; set; } = [];
}