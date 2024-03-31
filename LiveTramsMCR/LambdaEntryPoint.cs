using Microsoft.AspNetCore.Hosting;

namespace LiveTramsMCR;

/// <summary>
/// Class used as a entry point for Lambda function
/// </summary>
public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
{
    /// <inheritdoc />
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseStartup<Startup>();
    }
}