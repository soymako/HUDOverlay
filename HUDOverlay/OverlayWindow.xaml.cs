using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HUDOverlay {
    /// <summary>
    /// Lógica de interacción para OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;

        private List<Image> imagenes = new List<Image>(); 


        public OverlayWindow(List<Image> images, Dictionary<int, Point> imagespos, Dictionary<int, ScaleTransform> imagesScale) {
            InitializeComponent();
            this.Loaded += setWindowHitboxless;
            this.imagenes = images;
            LoadImages(images, imagespos, imagesScale);
        }

        private void LoadImages(List<Image> images, Dictionary<int, Point> imagespos, Dictionary<int, ScaleTransform> imagesScale) {
            foreach (Image image in imagenes) {
                Image img = new Image {
                    Source = image.Source,
                    Height = image.Height,
                    Width = image.Width,
                    Tag = image.Tag
                };
                ImageHolder.Children.Add(img);
                int imageId = (int) img.Tag;
                Canvas.SetLeft(img, imagespos[imageId].X);
                Canvas.SetTop(img, imagespos[imageId].Y);
                ScaleTransform scaleTransform = new ScaleTransform(imagesScale[imageId].ScaleX, imagesScale[imageId].ScaleY);
                img.RenderTransform = scaleTransform;
            }
        }

        private void setWindowHitboxless(object sender, RoutedEventArgs e) {
            var hwnd = new WindowInteropHelper(this).Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }
    }
}
