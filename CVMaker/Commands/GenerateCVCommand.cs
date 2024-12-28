using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using AngleSharp.Dom;
using CVMaker.Configuration;
using CVMaker.Context;
using CVMaker.Context.Models;
using CVMaker.ResponseFormats;
using CVMaker.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using Document = QuestPDF.Fluent.Document;
using Unit = QuestPDF.Infrastructure.Unit;

namespace CVMaker.Commands;

public class CvResponse
{
    public byte[] File { get; set; }
    public string FileName { get; set; }
}
public class GenerateCvCommand : IRequest<CvResponse>
{
    public string JobDescription { get; set; } = null!;
}

#pragma warning disable SKEXP0001
public class ChatCommandHandler : IRequestHandler<GenerateCvCommand, CvResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IContextAccessorService _contextAccessorService;
    private readonly IOptions<AIConfiguration> _options;
    private readonly Kernel _kernel;    
    private readonly IChatCompletionService _chatCompletionService;    
    public ChatCommandHandler(
        ApplicationDbContext dbContext,
        IContextAccessorService contextAccessorService,
        IOptions<AIConfiguration> options,
        Kernel kernel, IChatCompletionService chatCompletionService)
    {
        _dbContext = dbContext;
        _contextAccessorService = contextAccessorService;
        _options = options;
        _kernel = kernel;
        _chatCompletionService = chatCompletionService;
    }

    [Experimental("SKEXP0010")]
    public async Task<CvResponse> Handle(GenerateCvCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == _contextAccessorService.UserId, cancellationToken: cancellationToken);
        if (user is null) throw new Exception("User not found");
        
        var executionOptions = new OpenAIPromptExecutionSettings()
        {
            Temperature = .7,
            ResponseFormat = typeof(CvResponseFormat),
            ChatSystemPrompt = string.Join("\n",_options.Value.SystemPrompts),
            MaxTokens = 24000,
            Seed = 42
        };
        
        var res = await _chatCompletionService.GetChatMessageContentAsync(ComposePrompt(user, request.JobDescription), executionOptions,_kernel, cancellationToken);

        var cvResponseFormat = JsonSerializer.Deserialize<CvResponseFormat>(res.Content!)!;

        return new CvResponse
        {
            File = GeneratePdf(cvResponseFormat, user),
            FileName = $"{user.ContactInfo.Name.Replace(" ", "_")}_CV.pdf"
        };
    }

    private static string ComposePrompt(User user, string jobDescription)
    {
        return $"""
                Based on this job description: {jobDescription}
                and user information:
                Work Experience: {string.Join(", ", user.Experiences.Select(x=>x.ToString()))}
                Skills: {string.Join(", ", user.Skills)}
                Languages: {string.Join(", ", user.Languages)}
                Projects: {string.Join(", ", user.Projects.Select(x=>x.ToString()))}
                Education: {string.Join(", ", user.Education.Select(x=>x.ToString()))}
                Return the most relevant work experinces, skills, projects, Language, and Education that match the job description 
                Also generate a max 25 words profile phragraph based on the user information and job description
                Also generate a title based on the job description
                """;
    }

    private static byte[] GeneratePdf(CvResponseFormat cv, User user)
    {
        var document = Document.Create(container =>
        {
            container.Page(
                page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page
                        .Content()
                        .DefaultTextStyle(x => x.FontFamily("Arial"))
                        .Column(descriptor =>
                        {
                            page.Header().Element(BuildHeader);
                            descriptor.Item().ShrinkVertical().Element(BuildProfile);
                            descriptor.Item().ShrinkVertical().Element(BuildExperience);
                            descriptor.Item().ShrinkVertical().Element(BuildProjects);
                            descriptor.Item().ShrinkVertical().Element(BuildEducation);
                            descriptor.Item().ShrinkVertical().Element(BuildSkills);
                            descriptor.Item().ShrinkVertical().Element(BuildLanguages);
                        });
                }
            );
        });

        // document.ShowInCompanion();
        return document.GeneratePdf();


        void BuildHeader(IContainer container)
        {
            container.Column(columnDescriptor =>
            {
                columnDescriptor
                    .Item()
                    .PaddingVertical(4)
                    .ShowOnce()
                    .Text(user.ContactInfo.Name)
                    .FontSize(20)
                    .Bold()
                    .AlignLeft();

                columnDescriptor
                    .Item()
                    .PaddingVertical(4)
                    .ShowOnce()
                    .Text(user.ContactInfo.JobTitle)
                    .FontSize(14)
                    .AlignLeft();

                columnDescriptor
                    .Item()
                    .ShowOnce()
                    .PaddingVertical(4)
                    .PaddingRight(20)
                    .DefaultTextStyle(x => x.SemiBold())
                    .Row(row =>
                    {
                        row.RelativeItem().Text(user.ContactInfo.Email).FontSize(10).AlignLeft();
                        row.RelativeItem().Text(user.ContactInfo.LinkedIn).FontSize(10).AlignRight();
                    });

                columnDescriptor
                    .Item()
                    .ShowOnce()
                    .PaddingVertical(4)
                    .PaddingRight(20)
                    .DefaultTextStyle(x => x.SemiBold())
                    .Row(row =>
                    {
                        row.RelativeItem().Text(user.ContactInfo.Phone).FontSize(10);
                        row.RelativeItem().Text(user.ContactInfo.Github).FontSize(10).AlignRight();
                    });
            });
        }

        void BuildProfile(IContainer container)
        {
            container.Column(descriptor =>
            {
                descriptor
                    .Item()
                    .PaddingTop(20)
                    .BorderBottom(1)
                    .Text("PROFILE")
                    .FontSize(16)
                    .Bold();

                descriptor
                    .Item()
                    .PaddingVertical(8)
                    .Text(cv.Profile)
                    .FontSize(11);
            });
        }

        void BuildExperience(IContainer container)
        {
            container.Column(descriptor =>
            {
                descriptor
                    .Item()
                    .PaddingTop(20)
                    .BorderBottom(1)
                    .Text("EXPERIENCE")
                    .FontSize(16)
                    .Bold();

                foreach (var experience in cv.Experiences)
                {
                    descriptor
                        .Item()
                        .PaddingVertical(5)
                        .Column(
                            columnDescriptor =>
                            {
                                columnDescriptor
                                    .Item()
                                    .DefaultTextStyle(x => x.FontSize(14))
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Text(experience.Company)
                                            .Bold();

                                        row.RelativeItem()
                                            .Text(
                                                $"{experience.StartDate:MM/yyyy} - {(experience.EndDate is null ? "Present" : experience.EndDate.ToString()!)}")
                                            .Bold()
                                            .AlignRight();
                                    });

                                columnDescriptor
                                    .Item()
                                    .DefaultTextStyle(x => x.FontSize(12))
                                    .Text(experience.JobTitle);

                                columnDescriptor
                                    .Item()
                                    .DefaultTextStyle(x => x.FontSize(10))
                                    .Text(experience.Description)
                                    .AlignLeft();
                            }
                        );
                }
            });
        }

        void BuildProjects(IContainer container)
        {
            container.Column(descriptor =>
            {
                descriptor
                    .Item()
                    .PaddingTop(20)
                    .BorderBottom(1)
                    .Text("PROJECTS")
                    .FontSize(16)
                    .Bold();

                foreach (var project in cv.Projects)
                {
                    descriptor
                        .Item()
                        .PaddingVertical(5)
                        .Column(
                            columnDescriptor =>
                            {
                                columnDescriptor
                                    .Item()
                                    .DefaultTextStyle(x => x.FontSize(14))
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Text(project.Title)
                                            .Bold();

                                        row.RelativeItem()
                                            .Text(project.Date.ToString("MM/yyyy"))
                                            .Bold()
                                            .AlignRight();
                                    });

                                columnDescriptor
                                    .Item()
                                    .DefaultTextStyle(x => x.FontSize(12))
                                    .Text(project.Description)
                                    .AlignLeft();
                            }
                        );
                }
            });
        }

        void BuildEducation(IContainer container)
        {
            container.Column(descriptor =>
            {
                descriptor
                    .Item()
                    .PaddingTop(20)
                    .BorderBottom(1)
                    .Text("EDUCATION")
                    .FontSize(16)
                    .Bold();

                foreach (var education in cv.Education)
                {
                    descriptor
                        .Item()
                        .ShrinkVertical()
                        .PaddingVertical(4)
                        .Column(
                            columnDescriptor =>
                            {
                                columnDescriptor
                                    .Item()
                                    .ShrinkVertical()
                                    .Row(row =>
                                    {
                                        row.RelativeItem().Text(education.University).Bold();
                                        row.RelativeItem()
                                            .Text($"{education.StartDate:MM/yyyy} - {education.EndDate:MM/yyyy}").Bold()
                                            .AlignRight();
                                    });

                                columnDescriptor
                                    .Item()
                                    .ShrinkVertical()
                                    .Text(education.Degree);
                            }
                        );
                }
            });
        }

        void BuildSkills(IContainer container)
        {
            container.Column(descriptor =>
            {
                descriptor
                    .Item()
                    .PaddingTop(20)
                    .BorderBottom(1)
                    .Text("SKILLS")
                    .FontSize(16)
                    .Bold();

                descriptor
                    .Item()
                    .PaddingVertical(4)
                    .DefaultTextStyle(x => x.FontSize(12))
                    .Text(string.Join(", ", cv.Skills));
            });
        }

        void BuildLanguages(IContainer container)
        {
            container.Column(descriptor =>
            {
                descriptor
                    .Item()
                    .PaddingTop(20)
                    .BorderBottom(1)
                    .Text("LANGUAGES")
                    .FontSize(16)
                    .Bold();

                descriptor
                    .Item()
                    .PaddingVertical(4)
                    .DefaultTextStyle(x => x.FontSize(12))
                    .Text(string.Join(", ", cv.Languages));
            });
        }
    }
}