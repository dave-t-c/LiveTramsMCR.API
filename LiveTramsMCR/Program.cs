using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LiveTramsMCR;

/// <summary>
///     Configures Host Builder and acts as program entry point
/// </summary>
public static class Program
{
    /// <summary>
    ///     Program entry point
    /// </summary>
    /// <param name="args">Command line args. These are not used</param>
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}