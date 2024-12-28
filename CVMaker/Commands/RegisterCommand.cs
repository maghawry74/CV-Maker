using CVMaker.Context;
using CVMaker.Context.Models;
using MediatR;

namespace CVMaker.Commands;

public class RegisterCommand : IRequest<object>
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public ContactInfo ContactInfo { get; set; } = null!;
    public List<Project> Projects { get; set; } = [];
    public List<Education> Education { get; set; } = [];
    public List<WorkExperience> Experiences { get; set; } = [];
    public string[] Skills { get; set; } = [];
    public string[] Languages { get; set; } = [];
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, object>
{
    private readonly ApplicationDbContext _context;

    public RegisterCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Name,
            request.Email, 
            request.Password, 
            request.ContactInfo, 
            request.Projects,
            request.Education, 
            request.Skills, 
            request.Languages, 
            request.Experiences);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return new { Message = "User registered successfully" };
    }
}