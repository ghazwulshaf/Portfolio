using System.ComponentModel.DataAnnotations;

namespace GhazwulShaf.Models;

public class Contact
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = default!;

    public string Phone { get; set; } = default!;
    public string Address { get; set; } = default!;
    public List<ContactLink> Links { get; set; } = new List<ContactLink>();
}

public class ContactLink
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Icon { get; set; } = default!;
    
    [DataType(DataType.Url)]
    public string Url { get; set; } = default!;
}