using System;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Services;
using TfGM_API_Wrapper.Models.Stops;
using static System.AppDomain;

namespace TfGM_API_Wrapper;

/// <summary>
/// Builds and configures the application
/// </summary>
public class Startup
{
    /// <summary>
    /// Generates application builder using appSettings JSON.
    /// </summary>
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(CurrentDomain.BaseDirectory)
            .AddJsonFile("appSettings.json", true, true)
            .AddUserSecrets<ApiOptions>()
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    private IConfiguration Configuration { get; }
    
    /// <summary>
    /// Method called by runtime, used to add services to the container.
    /// This adds the required resources and models to be used for the program.
    /// </summary>
    /// <param name="services">Services for the Container</param>
    public void ConfigureServices(IServiceCollection services)
    {

        services.Configure<ApiOptions>(Configuration.GetSection("ApiOptions"));

        // ReSharper disable once SuspiciousTypeConversion.Global
        ResourcesConfig resourceConfig = new ResourcesConfig();
        Configuration.Bind("Resources", resourceConfig);

        // This is currently imported and set manually due to problems with it
        // working with Linux on Azure App Service, as it did not want to work
        // with a structured json.
        ApiOptions apiOptions = new ApiOptions
        {
            OcpApimSubscriptionKey = Configuration["OcpApimSubscriptionKey"]
        };
        services.AddSingleton(apiOptions);

        ResourceLoader resourceLoader = new ResourceLoader(resourceConfig);
        ImportedResources importedResources = resourceLoader.ImportResources();
        services.AddSingleton(importedResources);

        IStopsDataModel stopsDataModel = new StopsDataModel(importedResources);
        services.AddSingleton(stopsDataModel);

        IRequester serviceRequester = new ServiceRequester(apiOptions);
        IServicesDataModel servicesDataModel = new ServicesDataModel(importedResources, serviceRequester);
        services.AddSingleton(servicesDataModel);

        IJourneyPlanner journeyPlanner = new JourneyPlanner(importedResources.ImportedRoutes);
        IJourneyPlannerModel journeyPlannerModel = new JourneyPlannerModel(importedResources, journeyPlanner);
        services.AddSingleton(journeyPlannerModel);

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TfGM API Wrapper", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.EnableAnnotations();
        });
    }
    
    /// <summary>
    /// Configures HTTP request pipeline
    /// </summary>
    /// <param name="app">App being built</param>
    /// <param name="env">Host Environment being used.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TfGM_API_Wrapper v1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}