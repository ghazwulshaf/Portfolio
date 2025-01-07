namespace GhazwulShaf.Models;

public class Masterdata
{
    public Welcome Welcome { get; set; } = new Welcome();
    public List<ProjectType> Types { get; set; } = [];
}

public class Welcome
{
    public string Heading { get; set; } = default!;
    public string Caption { get; set; } = default!;
}

public class ProjectType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}