﻿using CGAwesome.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.Interfaces
{
    public interface IGenerateShowcase
    {
        void Generate(string fillBlock, bool transparency, string transparencyBlockName);
    }
}
