using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CVMaker.Context;
using CVMaker.Context.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CVMaker.Commands;

public class LoginCommand : IRequest<object?>
{
   public string Email { get; set; } = null!;
   public string Password { get; set; } = null!;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, object?>
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(ApplicationDbContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    public async Task<object?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken); 
        if(user is null || user.Password != request.Password)
        {
            return null;
        }
        
        return new {Token = GenerateToken(user,_configuration["Authentication:Key"]!)};
    }

    private static string GenerateToken(User user, string secret)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}