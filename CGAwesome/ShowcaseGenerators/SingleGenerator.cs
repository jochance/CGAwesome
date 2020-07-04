using CGAwesome.Enums;
using CGAwesome.ImageMeat;
using CGAwesome.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace CGAwesome.ShowcaseGenerators
{
    internal class SingleGenerator : ShowcaseGenerator
    {
        private System.Drawing.Bitmap ProcessedImage { get; }
        private Orientation Orientation { get; }
        private IExportMinecraftPackage PackageExporter { get; }
        public SingleGenerator(IExportMinecraftPackage exporter, System.Drawing.Bitmap processedImage, string packageName, string newVersion, string functionName, Orientation orientation) :
                base(exporter, packageName, newVersion, functionName)
        {
            ProcessedImage = processedImage;
            Orientation = orientation;
            PackageExporter = exporter;
        }

        public override void Generate(string fillBlock, bool transparency, string transparencyBlockName)
        {
            var packName = base.PackageName;
            var packRoot = base.PackRoot;
            var functionName = base.FunctionName;
            var newVersion = base.NewVersion;

            var fillBlockDictionary = MinecraftColorConvert.GetColorToMinecraftBlockDictionary(fillBlock, transparency, PackageExporter.GetPackageType() == ExportPackageType.Java, transparencyBlockName);

            base.FillCommands = ImageProcessing.GetFillCommandsFromImage(ProcessedImage, fillBlockDictionary, Orientation);

            Export();

            ZipFile.CreateFromDirectory($"{packRoot.FullName}", $"{packRoot.Parent.FullName}\\{packName}.mcpack");

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks";

            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
        }

        public override void Export()
        {
            PackageExporter.Cleanup(PackRoot, PackageName, FunctionName);

            if (String.IsNullOrEmpty(FillCommands.ToString()))
                throw new ArgumentNullException();

            PackageExporter.ExportFunction(FillCommands, PackRoot, FunctionName);
            PackageExporter.ExportPackage(PackRoot, PackageName, NewVersion);
        }
    }
}
