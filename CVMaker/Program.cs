using CVMaker.Commands;
using CVMaker.Configuration;
using CVMaker.Context;
using CVMaker.Extensions;
using CVMaker.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),o=>o.UseVector());
});

builder.Services.Configure<AIConfiguration>(builder.Configuration.GetSection(AIConfiguration.SectionName));

#pragma warning disable SKEXP0070
builder.Services.AddAI(builder.Configuration);
#pragma warning restore SKEXP0070
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IContextAccessorService, ContextAccessor>();
builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssemblyContaining<Program>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/login", async (IMediator mediator, [FromBody] LoginCommand login) =>
{
    var result = await mediator.Send(login);
    return result is null ? Results.Unauthorized() : Results.Ok(result);
}).AllowAnonymous();

app.MapPost("/register",
    async (IMediator mediator, [FromBody] RegisterCommand register) => await mediator.Send(register))
    .AllowAnonymous();

app.MapPut("/update-cv",
    async (IMediator mediator, [FromBody] UpdateUserCvCommand updateUserCvCommand) => Results.Ok(await mediator.Send(updateUserCvCommand)))
    .RequireAuthorization();

app.MapGet("/generate-cv", async (IMediator mediator,[FromQuery] string jobDescription) =>
{
    var result = await mediator.Send(new GenerateCvCommand{JobDescription = jobDescription});
    return Results.File(result.File, "application/pdf", result.FileName);
}).RequireAuthorization();

app.Run();
