using System;
using System.Threading.Tasks;

namespace ModInfoFileGenerator
{
    internal static class Program
    {
        internal static async Task Main(string[] args)
        {
            try
            {
                await App.RunAsync(args);
                Console.WriteLine("Process Complete: modinfo.json file has been created.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");
            }
        }
    }
}