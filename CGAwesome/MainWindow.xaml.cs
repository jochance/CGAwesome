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
using System.Text;
using System.Collections.Generic;

namespace CGAwesome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<System.Windows.Controls.Image, Bitmap> ProcessedImages;

        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\ProcessedImages"))
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\ProcessedImages");

            ProcessedImages = new Dictionary<System.Windows.Controls.Image, Bitmap>();
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(singleScaleX.Text), int.Parse(singleScaleY.Text), MainImage);
        }

        private void SetImageSource(System.Windows.Controls.Image imageControl, FileInfo sourceFile, Bitmap bitmap)
        {
            var tmpFileTarget = $"{AppDomain.CurrentDomain.BaseDirectory}\\ProcessedImages\\{sourceFile.Name}";

            if (File.Exists(tmpFileTarget))
                File.Delete(tmpFileTarget);

            bitmap.Save(tmpFileTarget);

            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.CacheOption = BitmapCacheOption.OnLoad;
            imageSource.UriSource = new Uri(tmpFileTarget);
            imageSource.EndInit();

            imageControl.Source = imageSource;
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
            var fillCommands = ImageProcessing.GetFillCommandsFromImage(ProcessedImages[MainImage], fillBlockDictionary, GetOrientationFromOptionGroup());

            ExportPackage(fillCommands, packName, functionName, optionNewVersion.Text, packRoot);

            ZipFile.CreateFromDirectory($"{packRoot.FullName}", $"{packRoot.Parent.FullName}\\{packName}.mcpack");

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\ExportedPacks";

            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
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

                        switch (line.Split(':')[0].Trim().Replace("\"", ""))
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

        private void ChooseImage1_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image1);
        }

        private void ChooseImage5_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image5);
        }

        private void ChooseImage2_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image2);
        }

        private void ChooseImage6_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image6);
        }

        private void ChooseImage3_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image3);
        }

        private void ChooseImage7_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image7);
        }

        private void ChooseImage4_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image4);
        }

        private void ChooseImage8_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image8);
        }

        private void ProcessImage(FileInfo sourceFile, int scaleWidth, int scaleHeight, System.Windows.Controls.Image imageControl = null)
        {
            using (Bitmap sourceImage = new Bitmap(sourceFile.FullName))
            using (Bitmap scaledImage = new Bitmap(sourceImage, scaleWidth, scaleHeight))
            using (Bitmap processedImage = ImageProcessing.ProcessToPalette(scaledImage, (bool)optionTransparencyMask.IsChecked))
            {
                if (imageControl != null)
                {
                    if (!ProcessedImages.ContainsKey(Image1))
                    {
                        ProcessedImages.Add(Image1, processedImage);
                    }
                    else
                    {
                        ProcessedImages[Image1] = processedImage;
                    }

                    SetImageSource(imageControl, sourceFile, processedImage);
                }
            }
        }

        #region "File Operations"
        private FileInfo SelectImageDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image files (*.bmp,*.jpg, *.jpeg, *.gif, *.png, *.tiff) | *.bmp; *.jpg; *.jpeg; *.gif; *.png; *.tiff;";

            openFileDialog.ShowDialog(this);

            FileInfo sourceFile = new FileInfo(openFileDialog.FileName);

            return sourceFile;
        }
        #endregion

        private void GenerateShowcase_Click(object sender, RoutedEventArgs e)
        {
            foreach (System.Windows.Controls.RadioButton rb in ShowcaseType.Children)
            {
                if ((bool)rb.IsChecked)
                    GenerateShowcase(rb.Content.ToString());
            }
        }

        private void GenerateShowcase(string showcaseType)
        {
            switch (showcaseType)
            {
                case "Massive Museum":
                    break;
                case "Large Longhouse":
                    break;
                case "Stately Studio":
                    break;
            }
        }
    }
}