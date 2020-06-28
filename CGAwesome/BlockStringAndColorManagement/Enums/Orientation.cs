using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.Enums
{
    /// <summary>
    /// Z, X, or Y plane (filled points will only vary by the plane not chosen because they all fall on that plane)
    /// </summary>
    public enum Orientation
    {
        NorthSouth = 1,
        EastWest = 2,
        FloorCeiling = 3
    }

}
