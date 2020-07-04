using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using CGAwesome.Enums;
using System.Collections.Generic;
using CGAwesome.ShowcaseGenerators;
using CGAwesome.Interfaces;
using CGAwesome.PackageExporters;
using CGAwesome.ImageMeat;

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

            ProcessedImages = new Dictionary<System.Windows.Controls.Image, Bitmap>();
        }

        #region "Private Methods"
        private void ProcessImage(FileInfo sourceFile, int scaleWidth, int scaleHeight, System.Windows.Controls.Image imageControl = null)
        {
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\ProcessedImages"))
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\ProcessedImages");

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

        private Orientation GetOrientationFromOptionGroup()
        {
            if ((bool)optionNorthSouth.IsChecked) return Orientation.NorthSouth;
            if ((bool)optionEastWest.IsChecked) return Orientation.EastWest;
            if ((bool)optionFloorCeiling.IsChecked) return Orientation.FloorCeiling;

            return Orientation.NorthSouth;
        }

        private void GenerateShowcase(string showcaseType)
        {
            IExportMinecraftPackage exporter;

            if ((bool)optionBedrock.IsChecked)
            {
                exporter = new BedrockExporter();
            }
            else
            {
                exporter = new JavaExporter();
            }

            switch (showcaseType)
            {
                case "Massive Museum":
                    new MuseumGenerator(exporter,
                                        optionPackName.Text,
                                        optionNewVersion.Text,
                                        optionFunctionName.Text)
                                        .Generate();
                    break;
                case "Large Longhouse":
                    new LonghouseGenerator(exporter,
                                           optionPackName.Text,
                                           optionNewVersion.Text,
                                           optionFunctionName.Text)
                                           .Generate();
                    break;
                case "Stately Studio":
                    new StudioGenerator(exporter,
                                        optionPackName.Text,
                                        optionNewVersion.Text,
                                        optionFunctionName.Text)
                                        .Generate();
                    break;
                case "Single":
                    new SingleGenerator(exporter,
                                        ProcessedImages[MainImage],
                                        optionNewVersion.Text,
                                        optionNewVersion.Text,
                                        optionFunctionName.Text,
                                        GetOrientationFromOptionGroup())
                                        .Generate(optionFillBlock.Text,
                                                (bool)optionTransparencyMask.IsChecked,
                                                optionTransparencyBlock.Text);
                    break;
            }
        }
        #endregion

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

        #region "Form Events"
        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(singleScaleX.Text), int.Parse(singleScaleY.Text), MainImage);
        }

        private void GenerateShowcase_Click(object sender, RoutedEventArgs e)
        {
            foreach (System.Windows.Controls.RadioButton rb in ShowcaseType.Children)
            {
                if ((bool)rb.IsChecked)
                {
                    GenerateShowcase(rb.Content.ToString());
                }
            }
        }

        private void OptionTransparencyMask_Checked(object sender, RoutedEventArgs e)
        {
            optionTransparencyBlock.IsEnabled = (bool)optionTransparencyMask.IsChecked;
        }

        private void BtnGenerateFillCommands_Click(object sender, RoutedEventArgs e)
        {
            GenerateShowcase("Single");
        }

        private void ChooseImage1_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image1);
        }

        private void ChooseImage2_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image2);
        }

        private void ChooseImage3_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image3);
        }

        private void ChooseImage4_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image4);
        }

        private void ChooseImage5_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image5);
        }

        private void ChooseImage6_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image6);
        }

        private void ChooseImage7_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image7);
        }

        private void ChooseImage8_Click(object sender, RoutedEventArgs e)
        {
            FileInfo sourceFile = SelectImageDialog();

            ProcessImage(sourceFile, int.Parse(showcaseScaleX.Text), int.Parse(showcaseScaleY.Text), Image8);
        }
        #endregion
    }
}