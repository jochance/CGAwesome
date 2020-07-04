using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.Interfaces
{
    public interface IExportMinecraftPackage
    {
        void Export(StringBuilder fillCommands, string packageName, string functionName, string newVersion, DirectoryInfo packRoot);
    }
}
