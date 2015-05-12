using AForge.Imaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Contrast_Stretch
{
    public partial class MainWindow : Window
    {
        private ImageStatistics stats;
        private string fileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                ImageOriginal.Source = new BitmapImage(new Uri(ofd.FileName));
                fileName = ofd.FileName;
                stats = new ImageStatistics(AForge.Imaging.Image.FromFile(ofd.FileName));
                drawHistogram(stats.Red.Values, 0);
                drawHistogram(stats.Green.Values, 1);
                drawHistogram(stats.Blue.Values, 2);
            }
        }

        private void drawHistogram(int[] values, int type)
        {
            PointCollection points = new PointCollection();
            points.Add(new Point(0, values.Max()));
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new Point(i, values.Max() - values[i]));
            }
            points.Add(new Point(values.Length - 1, values.Max()));
            switch (type)
            {
                case 0: PolygonRedOriginal.Points = points; break;
                case 1: PolygonGreenOriginal.Points = points; break;
                case 2: PolygonBlueOriginal.Points = points; break;
                case 3: PolygonRedEdited.Points = points; break;
                case 4: PolygonGreenEdited.Points = points; break;
                case 5: PolygonBlueEdited.Points = points; break;
            }
        }

        private void ButtonStretch_Click(object sender, RoutedEventArgs e)
        {
            if (ImageOriginal.Source == null)
            {
                MessageBox.Show("Please browse an image first");
                return;
            }
            else
            {
                System.Drawing.Bitmap BitmapOriginal = new System.Drawing.Bitmap(fileName);
                System.Drawing.Bitmap BitmapEdited = new System.Drawing.Bitmap(BitmapOriginal.Width, BitmapOriginal.Height);
                int[] maxAndMin = GetMaxMin(BitmapOriginal);
                for (int x = 0; x < BitmapOriginal.Width; x++)
                {
                    for (int y = 0; y < BitmapOriginal.Height; y++)
                    {
                        System.Drawing.Color color = BitmapOriginal.GetPixel(x, y);
                        int r = ((color.R - maxAndMin[1]) * 255 / (maxAndMin[0] - maxAndMin[1]));
                        int g = ((color.G - maxAndMin[1]) * 255 / (maxAndMin[0] - maxAndMin[1]));
                        int b = ((color.B - maxAndMin[1]) * 255 / (maxAndMin[0] - maxAndMin[1]));
                        System.Drawing.Color ColorEdited = System.Drawing.Color.FromArgb(r, g, b);
                        BitmapEdited.SetPixel(x, y, ColorEdited);
                    }
                }
                MemoryStream ms = new MemoryStream();
                BitmapEdited.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                ImageEdited.Source = bi;
                stats = new ImageStatistics(BitmapEdited);
                drawHistogram(stats.Red.Values, 3);
                drawHistogram(stats.Green.Values, 4);
                drawHistogram(stats.Blue.Values, 5);
            }
        }

        private int[] GetMaxMin(System.Drawing.Bitmap bitmap)
        {
            int[] maxAndMin = new int[] { 0, 255 };
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    if (maxAndMin[0] < r)
                    {
                        maxAndMin[0] = r;
                    }
                    if (maxAndMin[0] < g)
                    {
                        maxAndMin[0] = g;
                    }
                    if (maxAndMin[0] < b)
                    {
                        maxAndMin[0] = b;
                    }
                    if (maxAndMin[1] > r)
                    {
                        maxAndMin[1] = r;
                    }
                    if (maxAndMin[1] > g)
                    {
                        maxAndMin[1] = g;
                    }
                    if (maxAndMin[1] > b)
                    {
                        maxAndMin[1] = b;
                    }
                }
            }
            return maxAndMin;
        }
    }
}
