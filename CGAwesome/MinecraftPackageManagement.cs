using CGAwesome.PackageExporters;
using System;
using System.IO;
using System.Text;

namespace CGAwesome
{
    public static class MinecraftPackageManagement
    {
        public static void Cleanup(DirectoryInfo packRoot, string packageName, string functionName)
        {
            if (!Directory.Exists(packRoot.FullName)) Directory.CreateDirectory(packRoot.FullName);

            if (!Directory.Exists($"{packRoot.FullName}\\functions")) Directory.CreateDirectory($"{packRoot.FullName}\\functions");
            if (File.Exists($"{packRoot.FullName}\\functions\\{functionName}.mcfunction")) File.Delete($"{packRoot.FullName}\\functions\\{functionName}.mcfunction");
            if (File.Exists($"{packRoot.Parent.FullName}\\{packageName}.mcpack"))
            {
                File.Delete($"{packRoot.Parent.FullName}\\{packageName}.mcpack");
            }
        }

        public static void ExportJavaPackage(StringBuilder fillCommands, string packageName, string functionName, string newVersion, DirectoryInfo packRoot)
        {
            throw new NotImplementedException();
        }

        public static void ExportBedrockPackage(StringBuilder fillCommands, string packageName, string functionName, string newVersion, DirectoryInfo packRoot)
        {
            var exporter = new BedrockExporter();

            exporter.Export(fillCommands, packageName, functionName, newVersion, packRoot);
        }
    }
}
