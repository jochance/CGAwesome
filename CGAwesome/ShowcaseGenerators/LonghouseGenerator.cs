﻿using CGAwesome.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.ShowcaseGenerators
{
    internal class LonghouseGenerator : ShowcaseGenerator
    {
        public LonghouseGenerator(IExportMinecraftPackage exporter, string packageName, string newVersion, string functionName) :
                base(exporter, packageName, newVersion, functionName)
        {

        }

        public override void Export()
        {
            throw new NotImplementedException();
        }

        public override void Generate(string fillBlock, bool transparency, string transparencyBlockName)
        {
            throw new NotImplementedException();
        }
    }
}