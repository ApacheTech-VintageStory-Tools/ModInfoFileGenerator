try
{
    await Parser.Default
        .ParseArguments<PackagerCommandLineArgs>(args)
        .WithParsedAsync(RunAsync);
    Console.WriteLine("Process Complete: modinfo.json file has been created.");
}
catch (Exception e)
{
    Console.WriteLine($"{e.GetType()}: {e.Message}");
}

static async Task RunAsync(PackagerCommandLineArgs option)
{
    if (!Path.IsPathRooted(option.TargetPath))
        option.TargetPath = Path.Combine(Environment.CurrentDirectory, option.TargetPath);

    var assemblyFile = new FileInfo(option.TargetPath);

    if (!assemblyFile.Exists)
        throw new FileNotFoundException("No file was found at the given location.", option.TargetPath);

    if (!assemblyFile.Extension.Equals(".dll"))
        throw new DllNotFoundException("The selected file is not a .dll file.");

    var outDir = option.TargetDir ?? assemblyFile.DirectoryName!;
    var assembly = Assembly.LoadFile(assemblyFile.FullName);
    var modInfo = assembly.PopulateJsonDto(option.VersionType);
    var json = JsonConvert.SerializeObject(modInfo, Formatting.Indented);
    await File.WriteAllTextAsync(Path.Combine(outDir, "modinfo.json"), json);
}