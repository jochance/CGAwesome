using CGAwesome.Enums;
using System.IO;
using System.Text;

namespace CGAwesome.Interfaces
{
    public interface IExportMinecraftPackage
    {
        ExportPackageType GetPackageType();
        void ExportFunction(StringBuilder fillCommands, DirectoryInfo packRoot, string functionName);
        void ExportPackage(DirectoryInfo packRoot, string packageName, string newVersion);
        void Cleanup(DirectoryInfo packRoot, string packageName, string functionName);
    }
}
