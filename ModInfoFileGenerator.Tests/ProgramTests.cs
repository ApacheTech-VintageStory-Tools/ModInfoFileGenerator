using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ModInfoFileGenerator.Converters;
using ModInfoFileGenerator.Model;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ModInfoFileGenerator.Tests
{
    [TestFixture]
    [TestOf(typeof(Program))]
    public class ProgramTests
    {
        private readonly FileInfo _targetFile = new (
            @"C:\Users\Apache\source\repos\ApacheTech.VintageMods.WaypointExtensions\ApacheTech.VintageMods.WaypointExtensions\bin\Release\netstandard2.0\ApacheTech.VintageMods.WaypointExtensions.dll");

        [Test]
        public void Program_Should_When()
        {
            Assert.Pass("Sanity Check");
        }

        [Test]
        public void App_ShouldThrowException_WhenFileNotFound()
        {
            Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                await App.RunAsync("-a InvalidPath".Split(' '));
            });
        }

        [Test]
        public void App_ShouldThrowException_WhenFileMisnamed()
        {
            Assert.ThrowsAsync<DllNotFoundException>(async () =>
            {
                var fileToTest = _targetFile.FullName.Replace(".dll", ".deps.json");
                await App.RunAsync($"-a {fileToTest}".Split(' '));
            });
        }

        [Test]
        public void App_ShouldThrowException_WhenFileNotAMod()
        {
            Assert.ThrowsAsync<CustomAttributeFormatException>(async () =>
            {
                var fileToTest = _targetFile.FullName.Replace("WaypointExtensions.dll", "Core.dll");
                await App.RunAsync($"-a {fileToTest}".Split(' '));
            });
        }

        [Test]
        public async Task Program_ShouldCreateModInfo_WhenFileIsValid()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await Program.Main($"-a {_targetFile.FullName}".Split(' '));
            });

            var modInfoFile = new FileInfo($@"{_targetFile.DirectoryName}\modinfo.json");

            Assert.True(modInfoFile.Exists);
            Assert.True(modInfoFile.Length > 0);

            var json = await modInfoFile.OpenText().ReadToEndAsync();

            Assert.AreNotEqual("null", json);

            var modInfo = JsonConvert.DeserializeObject<ModInfoJsonDto>(json);

            Assert.NotNull(modInfo);

            var assembly = Assembly.LoadFile(_targetFile.FullName);
            var expected = assembly.PopulateJsonDto("static");

            Assert.AreEqual(expected.Type, modInfo.Type);
            Assert.AreEqual(expected.Name, modInfo.Name);
            Assert.AreEqual(expected.ModId, modInfo.ModId);
            Assert.AreEqual(expected.Side, modInfo.Side);
            Assert.AreEqual(expected.Description, modInfo.Description);
            Assert.AreEqual(expected.Version, modInfo.Version);
            Assert.AreEqual(expected.Authors, modInfo.Authors);
            Assert.AreEqual(expected.Contributors, modInfo.Contributors);
            Assert.AreEqual(expected.Website, modInfo.Website);
            Assert.AreEqual(expected.WorldConfig, modInfo.WorldConfig);
            Assert.AreEqual(expected.RequiredOnClient, modInfo.RequiredOnClient);
            Assert.AreEqual(expected.RequiredOnServer, modInfo.RequiredOnServer);
            Assert.AreEqual(expected.NetworkVersion, modInfo.NetworkVersion);
            Assert.AreEqual(expected.Dependencies.Count, modInfo.Dependencies.Count);
            for (var i = 0; i < expected.Dependencies.Count; i++)
            {
                Assert.AreEqual(expected.Dependencies[i].ModID, modInfo.Dependencies[i].ModID);
                Assert.AreEqual(expected.Dependencies[i].Version, modInfo.Dependencies[i].Version);
            }
        }
    }
}
