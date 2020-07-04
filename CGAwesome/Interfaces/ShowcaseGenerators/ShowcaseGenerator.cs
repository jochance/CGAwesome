using CGAwesome.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.ShowcaseGenerators
{
    internal abstract class ShowcaseGenerator : IGenerateShowcase
    {
        private IExportMinecraftPackage PackageExporter { get; set; }
        private StringBuilder FillCommands { get; set; }
        private string PackageName { get; }
        private string NewVersion { get; }
        private string FunctionName { get; }
        private DirectoryInfo PackRoot { get; }
        
        public ShowcaseGenerator(IExportMinecraftPackage exporter, string packageName, string newVersion, string functionName)
        {
            PackageExporter = exporter;
            PackageName = packageName;
            NewVersion = newVersion;
            FunctionName = functionName;
            PackRoot = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks\\{packageName}");
        }

        public abstract void Generate();

        public virtual void Export()
        {
            PackageExporter.Export(FillCommands, PackageName, FunctionName, NewVersion, PackRoot);
        }
    }
}
