namespace CVMaker.Configuration;

public class AIConfiguration
{
    public static string SectionName = "AI";
    public List<string> SystemPrompts { get; init; } = null!;
}