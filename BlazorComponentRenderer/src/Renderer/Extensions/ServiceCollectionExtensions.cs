using BlazorComponentRenderer.Renderer.Classes;
using BlazorComponentRenderer.Renderer.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorComponentRenderer.Renderer.Extensions;

/// <summary>
/// Provides extension methods to <see cref="IServiceCollection"/> for registering the ArcBlazor rendering services.
/// </summary>
public static class ServiceCollectionExtensions
{
    
    public static IServiceCollection AddBlazorComponentToStringRenderer(this IServiceCollection services)
    { 
        // Register the IComponentRenderer as scoped
        services.AddScoped<IComponentRenderer, ComponentRenderer>();

        // Register the IParameterConverter as scoped
        services.AddScoped<IParameterConverter, ParameterConverter>();

        // Register the IArcBlazorComponentRenderer as singleton
        services.AddScoped<IArcBlazorComponentRenderer, ArcBlazorComponentRenderer>();
        return services;
    }
}
