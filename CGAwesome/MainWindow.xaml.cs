using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.Compression;
using CGAwesome.Enums;
using CGAwesome.MinecraftColor;
using Color = System.Drawing.Color;
using System.Text;
using System.Collections.Generic;
using Point = System.Drawing.Point;
using System.Linq;
using System.Windows.Media;

namespace CGAwesome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap CurrentImage;
        //private MinecraftWindowImage _blockImage;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image files (*.bmp,*.jpg, *.jpeg, *.gif, *.png, *.tiff) | *.bmp; *.jpg; *.jpeg; *.gif; *.png; *.tiff;";

            openFileDialog.ShowDialog(this);

            FileInfo sourceFile = new FileInfo(openFileDialog.FileName);

            CurrentImage = new Bitmap(new Bitmap(openFileDialog.FileName), int.Parse(txtX.Text), int.Parse(txtY.Text));
            CurrentImage = ProcessToPalette(CurrentImage, (bool)optionTransparencyMask.IsChecked);

            displayedImage.Source = null;
                
            var tmpFileName = $"{sourceFile.FullName.Replace(sourceFile.Extension, "_modified" + sourceFile.Extension)}";
            if (File.Exists($"{sourceFile.FullName.Replace(sourceFile.Extension, "_modified" + sourceFile.Extension)}")) File.Delete($"{sourceFile.FullName.Replace(sourceFile.Extension, "_modified" + sourceFile.Extension)}");
            CurrentImage.Save(tmpFileName);

            //CachedBitmap imageSource = new CachedBitmap(processedBitmap, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            displayedImage.Source = new BitmapImage(new Uri(tmpFileName));
        }


        /// <summary>
        /// Export Pack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGenerateFillCommands_Click(object sender, RoutedEventArgs e)
        {
            var packName = optionPackName.Text;
            var packRoot = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks\\{packName}");
            var functionName = optionFunctionName.Text;

            Cleanup(packRoot, packName, functionName);

            var fillBlockDictionary = MinecraftColorConvert.GetColorToMinecraftBlockDictionary(optionFillBlock.Text, (bool)optionTransparencyMask.IsChecked, (bool)optionJava.IsChecked, optionTransparencyBlock.Text);
            var fillCommands = GetFillCommandsFromImage(CurrentImage, fillBlockDictionary, GetOrientationFromOptionGroup());

            ExportPackage(fillCommands, packName, functionName, optionNewVersion.Text, packRoot);

            ZipFile.CreateFromDirectory($"{packRoot.FullName}", $"{packRoot.Parent.FullName}\\{packName}.mcpack");

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks";

            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
        }

        private StringBuilder GetFillCommandsFromImage(Bitmap currentImage, Dictionary<Color, string> fillBlockDictionary, Orientation blockOrientation)
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


        private void Cleanup(DirectoryInfo packRoot, string packageName, string functionName)
        {
            if (!Directory.Exists(packRoot.FullName)) Directory.CreateDirectory(packRoot.FullName);

            if (!Directory.Exists($"{packRoot.FullName}\\functions")) Directory.CreateDirectory($"{packRoot.FullName}\\functions");
            if (File.Exists($"{packRoot.FullName}\\functions\\{functionName}.mcfunction")) File.Delete($"{packRoot.FullName}\\functions\\{functionName}.mcfunction");
            if (File.Exists($"{packRoot.Parent.FullName}\\{packageName}.mcpack"))
            {
                File.Delete($"{packRoot.Parent.FullName}\\{packageName}.mcpack");
            }
        }

        public Bitmap ProcessToPalette(Bitmap bitmap, bool transparencyFill, bool blackReplace = false)
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
                for (int y = bitmap.Height - 1; y >=0; y--)
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

                            if(GetDistance(c, col, transparencyFill) == 0)
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

        private static void ExportPackage(StringBuilder fillCommands, string packageName, string functionName, string newVersion, DirectoryInfo packRoot)
        {
            using (StreamWriter sw = new StreamWriter($"{packRoot.FullName}\\functions\\{functionName}.mcfunction"))
            {
                sw.Write(fillCommands.ToString());
            }
            //TODO: Open behavior_pack_manifest and update version? Implement save/import of other scripts? Function slots? /function cga1, cga2, etc?

            if (!File.Exists($"{packRoot.FullName}\\manifest.json"))
            {
                using (StreamReader sr = new StreamReader(new FileStream($"{AppDomain.CurrentDomain.BaseDirectory}\\Manifest Templates\\behavior_pack_manifest.json", FileMode.Open)))
                using (StreamWriter sw = new StreamWriter($"{packRoot.FullName}\\manifest.json"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        switch (line.Split(':')[0].Trim().Replace("\"",""))
                        {
                            case "uuid":
                                sw.WriteLine($"\"uuid\":\"{Guid.NewGuid().ToString()}\",");
                                break;
                            case "description":
                                sw.WriteLine($"\"description\":\"{packageName}\",");
                                break;
                            case "name":
                                sw.WriteLine($"\"name\":\"{packageName}\",");
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }
            }
            else
            {
                var templateManifestPath = $"{packRoot.FullName}\\manifest.json";
                var newManifestPath = $"{packRoot.FullName}\\newmanifest.json";
                var oldManifestPath = $"{packRoot.FullName}\\oldmanifest.json";

                if (File.Exists(oldManifestPath)) File.Delete(oldManifestPath);

                using (StreamReader sr = new StreamReader(new FileStream(templateManifestPath, FileMode.Open)))
                using (StreamWriter sw = new StreamWriter(newManifestPath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string version = newVersion;
                        switch (line.Split(':')[0].Trim().Replace("\"", ""))
                        {
                            case "version":
                                sw.WriteLine($"\"version\": [{version}],");
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }

                File.Replace(newManifestPath, templateManifestPath, oldManifestPath);
                File.Delete(oldManifestPath);
            }
        }

        private static StringBuilder GetFillCommands(Dictionary<Color, List<Point>> blockDictionary, Dictionary<Color, string> fillBlockDictionary)
        {
            blockDictionary = CompressBlockDictionary(blockDictionary);

            StringBuilder fillCommands = new StringBuilder();

            //look at compressed result to simplify what needs to happen

            foreach(var color in blockDictionary.Keys)
            {
                var firstPoint = blockDictionary[color].OrderBy(p => p.Y).OrderBy(p => p.X).First();
                var lastPoint = firstPoint;
                var firstY = firstPoint.Y;

                foreach (var point in blockDictionary[color].OrderBy(p => p.Y).OrderBy(p => p.X))
                {
                    if (point.Y != lastPoint.Y && point.X == lastPoint.X)
                    {
                        fillCommands.Append($"fill ~0~{firstY}~{point.X}~0~{point.Y}~{point.X} {fillBlockDictionary[color]}{Environment.NewLine}");
                        firstY = point.Y;
                    }
                    else
                    {
                        firstY = point.Y;
                    }

                    lastPoint = point;
                }
            }

            return fillCommands;
        }

        private static Dictionary<Color, List<Point>> CompressBlockDictionary(Dictionary<Color, List<Point>> dictionary)
        {
            

            var newDictionary = new Dictionary<Color, List<Point>>();

            foreach(Color c in dictionary.Keys)
            {
                var orderedPointList = dictionary[c].OrderBy(p => p.Y).OrderBy(p => p.X);
                var firstPoint = orderedPointList.First();
                var colorChangePoints = new List<Point>() { firstPoint };
                var finalPoint = orderedPointList.Last();
                var previousPoint = firstPoint;

                foreach (var point in orderedPointList)
                {
                    if (point == firstPoint) continue;

                    if((point.Y - previousPoint.Y) > 1 || point.X != previousPoint.X || point == finalPoint)
                    {
                        colorChangePoints.Add(previousPoint);
                        colorChangePoints.Add(point);
                        firstPoint = point;
                        previousPoint = point;
                    }
                    else if((point.Y - previousPoint.Y) == 1)
                    {
                        firstPoint = point;
                        previousPoint = point;
                    }
                    else
                    {
                        previousPoint = point;
                    }
                }

                newDictionary.Add(c, colorChangePoints);

                if (newDictionary[c].Count() % 2 == 0) throw new Exception("Something bad happened.");
            }

            return newDictionary;
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

        public Orientation GetOrientationFromOptionGroup()
        {
            if ((bool)optionNorthSouth.IsChecked) return Orientation.NorthSouth;
            if ((bool)optionEastWest.IsChecked) return Orientation.EastWest;
            if ((bool)optionFloorCeiling.IsChecked) return Orientation.FloorCeiling;

            return Orientation.NorthSouth;
        }

        private void OptionTransparencyMask_Checked(object sender, RoutedEventArgs e)
        {
            optionTransparencyBlock.IsEnabled = (bool)optionTransparencyMask.IsChecked;
        }

        private void Button_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
