using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommandLine;
using ModInfoFileGenerator.Converters;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global

[assembly: InternalsVisibleTo("ModInfoFileGenerator.Tests")]

namespace ModInfoFileGenerator
{
    internal class App
    {
        internal static async Task RunAsync(IEnumerable<string> args)
        {
            await Parser.Default
                .ParseArguments<PackagerCommandLineArgs>(args)
                .WithParsedAsync(AppAction);
        }

        private static async Task AppAction(PackagerCommandLineArgs option)
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
    }
}