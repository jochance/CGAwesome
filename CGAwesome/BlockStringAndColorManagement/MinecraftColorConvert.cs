using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;
using CGAwesome.Enums;

namespace CGAwesome.MinecraftColor
{
    using ColorToMinecraftColorIndex = Dictionary<Color, MinecraftColorIndex>;
    using ColorToMinecraftBlockDictionary = Dictionary<Color, string>;

    public static class MinecraftColorConvert
    {
        public static ColorToMinecraftColorIndex COLOR_PALETTE = new ColorToMinecraftColorIndex()
        {
            {Color.FromArgb(255, 25, 25, 25),MinecraftColorIndex.black},
            {Color.FromArgb(255, 51, 76, 178),MinecraftColorIndex.blue},
            {Color.FromArgb(255, 102, 76, 51),MinecraftColorIndex.brown},
            {Color.FromArgb(255, 76, 127, 153),MinecraftColorIndex.cyan},
            {Color.FromArgb(255, 76, 76, 76),MinecraftColorIndex.gray},
            {Color.FromArgb(255, 102, 127, 51),MinecraftColorIndex.green},
            {Color.FromArgb(255, 102, 153, 216),MinecraftColorIndex.light_blue},
            {Color.FromArgb(255, 127, 204, 25),MinecraftColorIndex.lime},
            {Color.FromArgb(255, 178, 76, 216),MinecraftColorIndex.magenta},
            {Color.FromArgb(255, 216, 127, 51),MinecraftColorIndex.orange},
            {Color.FromArgb(255, 242, 127, 165),MinecraftColorIndex.pink},
            {Color.FromArgb(255, 127, 63, 178),MinecraftColorIndex.purple},
            {Color.FromArgb(255, 153, 51, 51),MinecraftColorIndex.red},
            {Color.FromArgb(255, 153, 153, 153),MinecraftColorIndex.light_gray},
            {Color.FromArgb(255, 229, 229, 51),MinecraftColorIndex.yellow},
            {Color.FromArgb(255, 255, 255, 255),MinecraftColorIndex.white}
        };


        public static Dictionary<Color, string> GetColorToMinecraftBlockDictionary(string blockName, bool forTransparency, bool jsPrefix, string transparencyBlockName)
        {
            var d = new ColorToMinecraftBlockDictionary();
            var jsPrefixString = jsPrefix ? "minecraft:" : "";

            if (forTransparency) return new Dictionary<Color, string>() { { Color.FromArgb(25, 0, 0, 0), transparencyBlockName }, { Color.FromArgb(255,125,125,125), blockName } };

            if (Enum.GetNames(typeof(ColorIndexedBlocks)).Any(e => e == blockName))
            {
                foreach (var color in COLOR_PALETTE.Keys)
                {
                    d.Add(color, $"{jsPrefixString}{blockName} {((int)COLOR_PALETTE[color]).ToString()}");
                }
            }
            else
            {
                foreach (var color in COLOR_PALETTE.Keys)
                {
                    d.Add(color, $"{jsPrefixString}{COLOR_PALETTE[color].ToString()}_{blockName}");
                }
            }

            return d;
        }
    }
}
