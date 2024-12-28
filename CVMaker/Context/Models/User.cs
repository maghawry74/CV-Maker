namespace CVMaker.Context.Models;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public ContactInfo ContactInfo { get; set; } = null!;
    public List<Project> Projects { get; set; } = [];
    public List<Education> Education { get; set; } = [];
    public List<WorkExperience> Experiences { get; set; } = [];
    public string[] Skills { get; private set; } = [];
    public string[] Languages { get; private set; } = [];

    public static User Create(string name,
        string email, 
        string password, 
        ContactInfo contactInfo,
        List<Project> projects, 
        List<Education> education,
        string[] skills, 
        string[] languages, 
        List<WorkExperience> experiences) => new()

    {
        Id = Guid.NewGuid(),
        Name = name,
        Email = email,
        Password = password,
        ContactInfo = contactInfo,
        Projects = projects,
        Education = education,
        Skills = skills,
        Languages = languages,
        Experiences = experiences,
    };
    
    public void UpdateUserInfo(ContactInfo contactInfo, List<Project> projects, List<Education> education,
        string[] skills, string[] languages, List<WorkExperience> experiences)
    {
        ContactInfo = contactInfo;
        Projects = projects;
        Education = education;
        Skills = skills;
        Languages = languages;
        Experiences = experiences;
    }
}

public class WorkExperience
{
    public WorkExperience() { }
    public string Company { get;  set; } = null!;
    public string JobTitle { get;  set; } = null!;
    public DateTime StartDate { get;  set; }
    public DateTime? EndDate { get;  set; }
    public string Description { get;  set; } = null!;
    public override string ToString()
    {
        return $"Company: {Company}\nJob Title: {JobTitle}\nStart Date: {StartDate}\nEnd Date: {EndDate}\nDescription: {Description}";
    }
}
public class Project
{
    public Project() { }
    public string Title { get;  set; } = null!;
    public string Description { get;  set; } = null!;
    public DateTime Date { get; set; }
    public override string ToString()
    {
        return $"Title: {Title}\nDescription: {Description}\nDate: {Date}";
    }
}

public class ContactInfo
{
    public ContactInfo() { }
    public string Name { get;  set; } = null!;
    public string JobTitle { get;  set; } = null!;
    public string Address { get;  set; } = null!;
    public string Phone { get;  set; } = null!;
    public string Email { get;  set; } = null!;
    public string Github { get; set; } = null!;
    public string LinkedIn { get; set; } = null!;
    public override string ToString()
    {
        return $"Name: {Name}\n Job Title:{JobTitle}\nAddress: {Address}\nPhone: {Phone}\nEmail: {Email}\nGithub: {Github}\nLinkedIn: {LinkedIn}";
    }
}
public class Education
{
    public Education() { }
    public string University { get;  set; } = null!;
    public string Degree { get;  set; } = null!;
    public DateTime StartDate { get;  set; }
    public DateTime EndDate { get;  set; }
    public override string ToString()
    {
        return $"University: {University}\nDegree: {Degree}\nStart Date: {StartDate}\nEnd Date: {EndDate}";
    }
}