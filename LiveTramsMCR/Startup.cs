using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using static System.AppDomain;

namespace LiveTramsMCR;

/// <summary>
///     Builds and configures the application
/// </summary>
public class Startup
{
    /// <summary>
    ///     Generates application builder using appSettings JSON.
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
    ///     Method called by runtime, used to add services to the container.
    ///     This adds the required resources and models to be used for the program.
    /// </summary>
    /// <param name="services">Services for the Container</param>
    public void ConfigureServices(IServiceCollection services)
    {
        var baseUrls = new BaseUrls();
        Configuration.Bind("BaseUrls", baseUrls);

        // This is currently imported and set manually due to problems with it
        // working with Linux on Azure App Service, as it did not want to work
        // with a structured json.
        var apiOptions = new ApiOptions
        {
            OcpApimSubscriptionKey = Configuration["OcpApimSubscriptionKey"], BaseRequestUrls = baseUrls
        };
        services.AddSingleton(apiOptions);

        var mongoClient = new MongoClient(Configuration["CosmosConnectionString"]);
        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(Configuration["aws-region-name"])
        };
        
        var dynamoDbClient = new AmazonDynamoDBClient(dynamoDbConfig);
        var dynamoDbContext = new DynamoDBContext(dynamoDbClient);

        var migrationMode = Environment.GetEnvironmentVariable(AppConfiguration.MigrationModeVariable);
        if (migrationMode == AppConfiguration.MigrationModeCreateValue)
        {
            var synchronizer = new Synchronizer();
            var synchronizationRequest = new SynchronizationRequest
            {
                MongoClient = mongoClient,
                TargetDbName = AppConfiguration.DatabaseName,
                StopsCollectionName = AppConfiguration.StopsCollectionName,
                StopsPath = AppConfiguration.StopsPath,
                StopsV2CollectionName = AppConfiguration.StopsV2CollectionName,
                StopsV2Path = AppConfiguration.StopsV2Path,
                RouteTimesCollectionName = AppConfiguration.RouteTimesCollectionName,
                RouteTimesPath = AppConfiguration.RouteTimesPath,
                RoutesCollectionName = AppConfiguration.RoutesCollectionName,
                RoutesPath = AppConfiguration.RoutesPath,
                RoutesV2CollectionName = AppConfiguration.RoutesV2CollectionName,
                RoutesV2Path = AppConfiguration.RoutesV2Path
            };
            synchronizer.SynchronizeStaticData(synchronizationRequest).Wait();
        }
        
        
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var stopsMongoCollection = db.GetCollection<Stop>(AppConfiguration.StopsCollectionName);
        var stopsV2MongoCollection = db.GetCollection<StopV2>(AppConfiguration.StopsV2CollectionName);
        var routesMongoCollection = db.GetCollection<Route>(AppConfiguration.RoutesCollectionName);
        var routesV2MongoCollection = db.GetCollection<RouteV2>(AppConfiguration.RoutesV2CollectionName);
        var routeTimesMongoCollection = db.GetCollection<RouteTimes>(AppConfiguration.RouteTimesCollectionName);
        
        IStopsRepository stopsRepository = new StopsRepository(stopsMongoCollection, dynamoDbContext);
        IStopsRepositoryV2 stopsRepositoryV2 = new StopsRepositoryV2(stopsV2MongoCollection);
        IRouteRepository routeRepository = new RouteRepository(routesMongoCollection, routeTimesMongoCollection, dynamoDbContext);
        IRouteRepositoryV2 routeRepositoryV2 = new RouteRepositoryV2(routesV2MongoCollection, stopsRepositoryV2);

        IStopsDataModel stopsDataModel = new StopsDataModel(stopsRepository);
        services.AddSingleton(stopsDataModel);

        IStopsDataModelV2 stopsDataModelV2 = new StopsDataModelV2(stopsRepositoryV2);
        services.AddSingleton(stopsDataModelV2);

        IRequester serviceRequester = new ServiceRequester(apiOptions);
        IServicesDataModel servicesDataModel = new ServicesDataModel(stopsRepository, serviceRequester);
        services.AddSingleton(servicesDataModel);

        IJourneyPlanner journeyPlanner = new JourneyPlanner(routeRepository);
        IJourneyPlannerModel journeyPlannerModel = new JourneyPlannerModel(stopsRepository, journeyPlanner);
        services.AddSingleton(journeyPlannerModel);

        IRoutesDataModelV2 routesDataModelV2 = new RoutesDataModelV2(routeRepositoryV2);
        services.AddSingleton(routesDataModelV2);

        var stopLookupV2 = new StopLookupV2(stopsRepositoryV2);
        IJourneyPlannerV2 journeyPlannerV2 = new JourneyPlannerV2(routeRepository, routeRepositoryV2);
        var journeyVisualiserV2 = new JourneyVisualiserV2();
        var nextServiceIdentifierV2 = new NextServiceIdentifierV2();
        IRequester requester = new ServiceRequester(apiOptions);
        var serviceProcessor = new ServiceProcessor(requester, stopsRepository);
        IJourneyPlannerModelV2 journeyPlannerModelV2 = new JourneyPlannerModelV2(
            stopLookupV2,
            journeyPlannerV2,
            journeyVisualiserV2,
            nextServiceIdentifierV2,
            serviceProcessor);

        services.AddSingleton(journeyPlannerModelV2);

        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LiveTramsMCR", Version = "v1"
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.EnableAnnotations();
        });
    }

    /// <summary>
    ///     Configures HTTP request pipeline
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