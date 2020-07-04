using CGAwesome.Enums;
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
        private IExportMinecraftPackage PackageExporter { get; }
        public StringBuilder FillCommands { get; set; }
        public string PackageName { get; }
        public string NewVersion { get; }
        public string FunctionName { get; }
        public DirectoryInfo PackRoot { get; }
        public ExportPackageType PackageType { get; }
        string FillBlock { get; }
        bool Transparency { get; }
        string TransparencyBlockName { get; }

        public ShowcaseGenerator(IExportMinecraftPackage exporter, string packageName, string newVersion, string functionName)
        {
            PackageExporter = exporter;
            PackageName = packageName;
            PackageType = exporter.GetPackageType();
            NewVersion = newVersion;
            FunctionName = functionName;
            PackRoot = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks\\{packageName}");
        }

        public abstract void Generate(string fillBlock, bool transparency, string transparencyBlockName);

        public abstract void Export();
    }
}
