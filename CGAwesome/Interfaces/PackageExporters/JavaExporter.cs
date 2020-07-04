using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAwesome.Interfaces;

namespace CGAwesome.PackageExporters
{
    public class JavaExporter : IExportMinecraftPackage
    {
        public void Export(StringBuilder fillCommands, string packageName, string functionName, string newVersion, DirectoryInfo packRoot)
        {
            throw new NotImplementedException();
        }
    }
}
