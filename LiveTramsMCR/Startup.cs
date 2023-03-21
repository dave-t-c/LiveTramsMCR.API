using System;
using System.IO;
using System.Reflection;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using static System.AppDomain;

namespace LiveTramsMCR;

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

        var mongoClient = new MongoClient(Configuration["CosmosConnectionString"]);
        var db = mongoClient.GetDatabase("livetramsmcr");
        var stopsMongoCollection = db.GetCollection<Stop>("stops");
        IStopsRepository stopsRepository = new StopsRepository(stopsMongoCollection);
        IStopsDataModel stopsDataModel = new StopsDataModel(stopsRepository);
        services.AddSingleton(stopsDataModel);

        IRequester serviceRequester = new ServiceRequester(apiOptions);
        IServicesDataModel servicesDataModel = new ServicesDataModel(stopsRepository, serviceRequester);
        services.AddSingleton(servicesDataModel);

        IRouteRepository routeRepository = new RouteRepository();
        IJourneyPlanner journeyPlanner = new JourneyPlanner(routeRepository);
        IJourneyPlannerModel journeyPlannerModel = new JourneyPlannerModel(stopsRepository, journeyPlanner);
        services.AddSingleton(journeyPlannerModel);

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "LiveTramsMCR", Version = "v1" });
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LiveTramsMCR v1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}