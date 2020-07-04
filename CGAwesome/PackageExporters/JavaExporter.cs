using System;
using System.IO;
using System.Text;
using CGAwesome.Enums;
using CGAwesome.Interfaces;

namespace CGAwesome.PackageExporters
{
    public class JavaExporter : IExportMinecraftPackage
    {
        public void Cleanup(DirectoryInfo packRoot, string packageName, string functionName)
        {
            throw new NotImplementedException();
        }

        public void ExportFunction(StringBuilder fillCommands, DirectoryInfo packRoot, string functionName)
        {
            throw new NotImplementedException();
        }

        public void ExportPackage(DirectoryInfo packRoot, string packageName, string newVersion)
        {
            throw new NotImplementedException();
        }

        public ExportPackageType GetPackageType()
        {
            return ExportPackageType.Java;
        }
    }
}
