using CGAwesome.Enums;
using CGAwesome.ImageMeat;
using CGAwesome.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.ShowcaseGenerators
{
    internal class StudioGenerator : ShowcaseGenerator
    {
        List<Bitmap> ImageFeatures { get; }
        IExportMinecraftPackage PackageExporter { get; }
        int StudioExtents { get; }
        string WallMaterial { get; }
        string FramingMaterial { get; }
        string FoundationMaterial { get; }
        string FloorMaterial { get; }

        public StudioGenerator(IExportMinecraftPackage exporter, Dictionary<System.Windows.Controls.Image, Bitmap> processedImages, string packageName, string newVersion, string functionName) :
                base(exporter, packageName, newVersion, functionName)
        {
            StudioExtents = 120;
            WallMaterial = "oak_planks";
            FramingMaterial = "dark_oak_planks";
            FoundationMaterial = "mossy_cobblestone";
            FloorMaterial = "cobblestone";

            ImageFeatures = new List<Bitmap>();

            foreach(var imageControl in processedImages.Keys)
            {
                if(imageControl.Name == "Image1" ||
                   imageControl.Name == "Image2")
                {
                    ImageFeatures.Add(processedImages[imageControl]);
                }
            }

            PackageExporter = exporter;
        }

        public override void Export()
        {
            var packName = base.PackageName;
            var packRoot = base.PackRoot;

            ZipFile.CreateFromDirectory($"{packRoot.FullName}", $"{packRoot.Parent.FullName}\\{packName}.mcpack");

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks";

            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
        }

        public override void Generate(string fillBlock, bool transparency, string transparencyBlockName)
        {
            StringBuilder floorFillCommands = GetFloorFillCommands(FloorMaterial);
            StringBuilder foundationFillCommands = GetFoundationFillCommands(FoundationMaterial);
            StringBuilder wallFillCommands = GetWallFillCommands(WallMaterial);
            StringBuilder framingFillCommands = GetFramingFillCommands(FramingMaterial);

            foreach(var image in ImageFeatures)
            {
                StringBuilder fillCommands = new StringBuilder();

                var fillBlockDictionary = MinecraftColorConvert.GetColorToMinecraftBlockDictionary(fillBlock, transparency, PackageExporter.GetPackageType() == ExportPackageType.Java, transparencyBlockName);

                fillCommands = ImageProcessing.GetFillCommandsFromImage(image, fillBlockDictionary, Orientation.NorthSouth);
            }

        }

        private StringBuilder GetFramingFillCommands(string framingMaterial)
        {
            throw new NotImplementedException();
        }

        private StringBuilder GetWallFillCommands(string wallMaterial)
        {
            StringBuilder fillCommands = new StringBuilder();

            var halfExtent = StudioExtents / 2;

            fillCommands.Append($"fill ~-{halfExtent}~0~-{halfExtent}~-{halfExtent}~{StudioExtents}~{halfExtent} {wallMaterial}");
            fillCommands.Append($"fill ~-{halfExtent}~0~-{halfExtent}~{halfExtent}~{StudioExtents}~-{halfExtent} {wallMaterial}");
            fillCommands.Append($"fill ~{halfExtent}~0~{halfExtent}~{halfExtent}~{StudioExtents}~-{halfExtent} {wallMaterial}");
            fillCommands.Append($"fill ~{halfExtent}~0~{halfExtent}~-{halfExtent}~{StudioExtents}~{halfExtent} {wallMaterial}");

            return fillCommands;
        }

        private StringBuilder GetFoundationFillCommands(string foundationMaterial)
        {
            StringBuilder fillCommands = new StringBuilder();

            var halfExtent = StudioExtents / 2;

            fillCommands.Append($"fill ~-{halfExtent}~-4~-{halfExtent}~{halfExtent}~-4~{halfExtent} {foundationMaterial}");
            fillCommands.Append($"fill ~-{halfExtent}~-3~-{halfExtent}~{halfExtent}~-3~{halfExtent} {foundationMaterial}");
            fillCommands.Append($"fill ~-{halfExtent}~-2~-{halfExtent}~{halfExtent}~-2~{halfExtent} {foundationMaterial}");
            fillCommands.Append($"fill ~-{halfExtent}~-1~-{halfExtent}~{halfExtent}~-1~{halfExtent} {foundationMaterial}");

            return fillCommands;
        }

        private StringBuilder GetFloorFillCommands(string floorMaterial)
        {
            StringBuilder fillCommands = new StringBuilder();

            var halfExtent = StudioExtents / 2;

            fillCommands.Append($"fill ~-{halfExtent}~0~-{halfExtent}~{halfExtent}~0~{halfExtent} {floorMaterial}");

            return fillCommands;
        }
    }
}
