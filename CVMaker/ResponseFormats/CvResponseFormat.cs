using CVMaker.Context.Models;

namespace CVMaker.ResponseFormats;

public class CvResponseFormat
{
    public string Profile { get; set; } = null!;
    public string Title { get; set; } = null!;
    public List<Project> Projects { get;  set; } = null!;
    public List<Education> Education { get;  set; } = null!;
    public List<WorkExperience> Experiences { get;  set; } = null!;
    public List<string> Skills { get;  set; } = null!;
    public List<string> Languages { get;  set; } = null!;
}