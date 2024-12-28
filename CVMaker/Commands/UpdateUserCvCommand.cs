using CVMaker.Context;
using CVMaker.Context.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CVMaker.Commands;


public class UpdateUserCvCommand : IRequest<object>
{
    public ContactInfo ContactInfo { get; set; } = null!;
    public List<Project> Projects { get; set; } = [];
    public List<Education> Education { get; set; } = [];
    public List<WorkExperience> Experiences { get; set; } = [];
    public string[] Skills { get; set; } = [];
    public string[] Languages { get; set; } = [];
}

public class UpdateUserCvCommandHandler : IRequestHandler<UpdateUserCvCommand, object>
{
    private readonly ApplicationDbContext _context;

    public UpdateUserCvCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> Handle(UpdateUserCvCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == _context.UserId, cancellationToken: cancellationToken);
        if (user is null) return new { Message = "User not found" };
        
        user.UpdateUserInfo(request.ContactInfo, request.Projects, request.Education, request.Skills, request.Languages, request.Experiences);
        
        await _context.SaveChangesAsync(cancellationToken);
        return new { Message = "CV updated successfully" };
    }
}