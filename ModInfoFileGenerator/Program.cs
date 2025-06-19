using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ModInfoFileGenerator.Converters;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

/// <summary>
///     The main entry point for the mod info file generator application.
///     Configures the host, logging, and dependency injection container.
/// </summary>
try
{
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton<IModInfoJsonDtoConverter, ModInfoJsonDtoConverter>();
            services.AddSingleton<App>();
        })
        .Build();

    var app = host.Services.GetRequiredService<App>();
    await app.RunAsync(args);
}
finally
{
    Log.CloseAndFlush();
}