using CGAwesome.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.ShowcaseGenerators
{
    internal class StudioGenerator : ShowcaseGenerator
    {
        public StudioGenerator(IExportMinecraftPackage exporter, string packageName, string newVersion, string functionName) :
                base(exporter, packageName, newVersion, functionName)
        {

        }

        public override void Generate()
        {
            throw new NotImplementedException();
        }

    }
}
