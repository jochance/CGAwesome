using CGAwesome.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.ShowcaseGenerators
{
    internal class MuseumGenerator : ShowcaseGenerator
    {
        public MuseumGenerator(IExportMinecraftPackage exporter, string packageName, string newVersion, string functionName) : 
            base(exporter, packageName, newVersion, functionName)
        {

        }

        public override void Generate()
        {

            base.Export();
        }
    }
}
