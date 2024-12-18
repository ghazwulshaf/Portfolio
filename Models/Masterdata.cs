namespace GhazwulShaf.Models;

public class Masterdata
{
    public List<ProjectType> Types { get; set; } = [];
}

public class ProjectType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}