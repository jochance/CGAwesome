using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CGAwesome.Enums;

namespace CGAwesome.ImageMeat
{
    public static class ImageProcessing
    {
        public static StringBuilder GetFillCommandsFromImage(Bitmap currentImage, Dictionary<Color, string> fillBlockDictionary, Orientation blockOrientation)
        {
            currentImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            StringBuilder fillCommands = new StringBuilder();

            for (int y = currentImage.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < currentImage.Width; x++)
                {
                    int colorEndX = x;

                    Color color = currentImage.GetPixel(y, x);

                    for (int x2 = x; x2 < currentImage.Width; x2++)
                    {
                        colorEndX = x2;

                        if (x2 == currentImage.Width - 1) break;

                        if (currentImage.GetPixel(y, x2) != color) break;
                    }

                    switch (blockOrientation)
                    {
                        case Orientation.EastWest:
                            fillCommands.Append($"fill ~{y}~{x}~0~{y}~{colorEndX}~0 {fillBlockDictionary[color]}{Environment.NewLine}");
                            break;
                        case Orientation.NorthSouth:
                            fillCommands.Append($"fill ~0~{x}~{y}~0~{colorEndX}~{y} {fillBlockDictionary[color]}{Environment.NewLine}");
                            break;
                        case Orientation.FloorCeiling:
                            fillCommands.Append($"fill ~{x}~0~{y}~{colorEndX}~0~{y} {fillBlockDictionary[color]}{Environment.NewLine}");
                            break;
                    }

                    x = (colorEndX == currentImage.Width - 1) ? x : colorEndX - 1;
                }
            }

            currentImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return fillCommands;
        }

        public static Bitmap ProcessToPalette(Bitmap bitmap, bool transparencyFill, bool blackReplace = false)
        {
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Bitmap newBitmap = new Bitmap(bitmap);

            HashSet<Color> colorSet = new HashSet<Color>();
            StringBuilder bld = new StringBuilder();

            if (!transparencyFill)
            {
                foreach (var key in MinecraftColorConvert.COLOR_PALETTE.Keys) colorSet.Add(key);
            }

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = bitmap.Height - 1; y >= 0; y--)
                {
                    Color nearest = MinecraftColorConvert.BLACK;
                    Color secondNearestColor = nearest;

                    int min = 500000000;

                    if (!transparencyFill)
                    {
                        foreach (var col in colorSet)
                        {
                            Color c = bitmap.GetPixel(x, y);

                            int secondNearestColorDistance = 0;
                            int minStart = min;

                            if (GetDistance(c, col, transparencyFill) == 0)
                            {
                                nearest = col;
                            }
                            else
                            {
                                min = min > GetDistance(c, col, transparencyFill) ? GetDistance(c, col, transparencyFill) : min;

                                if (minStart != min)
                                {
                                    secondNearestColor = nearest;
                                    secondNearestColorDistance = GetDistance(nearest, col, transparencyFill);
                                    nearest = col;
                                }
                            }

                            if (blackReplace && (secondNearestColorDistance <= 500 && nearest == Color.FromArgb(255, 25, 25, 25))) nearest = secondNearestColor;
                        }

                        newBitmap.SetPixel(x, y, nearest);
                    }
                    else
                    {
                        Color c = bitmap.GetPixel(x, y);

                        if (c.A < 100 || GetDistance(c, MinecraftColorConvert.BLACK) > GetDistance(c, MinecraftColorConvert.WHITE))
                        {
                            newBitmap.SetPixel(x, y, MinecraftColorConvert.WHITE);
                        }
                        else
                        {
                            newBitmap.SetPixel(x, y, MinecraftColorConvert.BLACK);
                        }
                    }
                }
            }

            newBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return newBitmap;
        }

        public static int GetDistance(Color current, Color match, bool useAlpha = false)
        {
            int redDifference;
            int greenDifference;
            int blueDifference;
            int alphaDifference;

            redDifference = current.R - match.R;
            greenDifference = current.G - match.G; //we see green better?
            blueDifference = current.B - match.B;

            if (useAlpha)
            {
                alphaDifference = current.A - match.A;

                return alphaDifference * alphaDifference + redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
            }

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }

    }
}
