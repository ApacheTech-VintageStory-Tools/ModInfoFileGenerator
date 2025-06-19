/// <summary>
///     The main entry point for the mod info file generator application.
///     Configures the host, logging, and dependency injection container.
/// </summary>
try
{
    var host = App.CreateHost(args);
    var app = host.Services.GetRequiredService<App>();
    await app.RunAsync(args);
}
finally
{
    Log.CloseAndFlush();
}