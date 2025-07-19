/// <summary>
///     The main entry point for the mod info file generator application.
///     Configures the host, logging, and dependency injection container.
/// </summary>
try
{
    await App.RunAsync(args);
}
finally
{
    Log.CloseAndFlush();
}