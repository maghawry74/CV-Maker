using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;

namespace CVMaker.Extensions;

public static class AIExtensions
{
    [Experimental("SKEXP0070")]
    public static IServiceCollection AddAI(this IServiceCollection services,IConfiguration configuration)
    {
        var endpoint = new Uri("Ollama End Point");
        
        services.AddOpenAIChatCompletion(
            modelId: "Model ID",
            endpoint: endpoint 
            );

        services.AddKernel();

        return services;
    }
}