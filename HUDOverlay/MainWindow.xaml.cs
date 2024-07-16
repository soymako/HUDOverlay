using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace HUDOverlay {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window {

        private Dictionary<int, Point> imagesPos = new Dictionary<int, Point>();
        private Dictionary<int, ScaleTransform> imageScale = new Dictionary<int, ScaleTransform>();
        private List<Image> images = new List<Image>();
        private Image currentImage = null;
        private Image imageToScale = null;

        private Point mousePos;
        private Point clickedPos;

        private Point scaleMousePos;
        private Point scaleClickedPos;
        private Point scaledResult;


        public MainWindow() {
            InitializeComponent();
            overlayOpener.Click += onOverlayOpenerClicked;
            imageImporter.Click += onImageImporterClick;
            this.MouseMove += onMouseMove;
            this.MouseDown += onMouseClick;
            this.MouseUp += onMouseClick;
            this.MouseDoubleClick += onLeftDoubleClick;
            deleteAllImages.Click += onDeleteAllImages;
        }

        private void onDeleteAllImages(object sender, RoutedEventArgs e) {
            foreach (Image img in images) {
                imagesCanvas.Children.Remove(img);
            }
            images.Clear();
            imagesPos.Clear();
            imageScale.Clear();
        }

        private void onLeftDoubleClick(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                if (currentImage != null) {
                    imagesCanvas.Children.Remove(currentImage);
                    currentImage = null;
                }
            }
        }

        private void onMouseClick(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) leftClickPressedHandler(e);
            if (e.LeftButton == MouseButtonState.Released) leftClickReleasedHandler(e);

            if (e.RightButton == MouseButtonState.Pressed) rightClickPressedHandler(e);
            if (e.RightButton == MouseButtonState.Released) rightClickReleasedHandler(e);
        }

        private void leftClickPressedHandler(MouseButtonEventArgs e) {
        }

        private void leftClickReleasedHandler(MouseButtonEventArgs e) {
            currentImage = null;
        }

        private void rightClickPressedHandler(MouseButtonEventArgs e) {
            scaleClickedPos = e.GetPosition(this);
        }

        private void rightClickReleasedHandler(MouseButtonEventArgs e) {
            scaleClickedPos = new Point(0, 0);
            scaledResult = new Point(0, 0);
        }

        private void onMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                mousePos = e.GetPosition(this);
                if (currentImage != null) {
                    int imageId = (int) currentImage.Tag;
                    if (imagesPos.ContainsKey(imageId)) {
                        imagesPos[imageId] = new Point(mousePos.X - clickedPos.X, mousePos.Y - clickedPos.Y);
                        Canvas.SetLeft(currentImage, mousePos.X - clickedPos.X);
                        Canvas.SetTop(currentImage, mousePos.Y - clickedPos.Y);
                    }
                }
            }
            if (e.RightButton == MouseButtonState.Pressed) {
                scaleMousePos = e.GetPosition(this);
                if (imageToScale != null) {
                    int imageId = (int)imageToScale.Tag;
                    if (imageScale.ContainsKey(imageId)) {
                        double scaleX = Math.Max(.1, scaleMousePos.X / imageToScale.Width);
                        double scaleY = Math.Max(.1, scaleMousePos.Y / imageToScale.Height);
                        ScaleTransform scaleTransform = new ScaleTransform(scaleX, scaleY);
                        imageScale[imageId] = scaleTransform;
                        imageToScale.RenderTransform = scaleTransform;
                    }
                }
            }
        }

        private void onOverlayOpenerClicked(object sender, RoutedEventArgs e) {
            if (images.Count == 0) {
                MessageBox.Show("No hay imagenes");
                return;
            }
            OverlayWindow overlayWindow = new OverlayWindow(images, imagesPos, imageScale);
            overlayWindow.Show();
        }

        private void onImageImporterClick(object sender, RoutedEventArgs e) {
            OpenFileDialog selector = new OpenFileDialog();
            selector.RestoreDirectory = true;
            selector.Filter = "PNG files (*.png) | *.png";
            if (selector.ShowDialog() == true) {
                loadImage(selector.FileName);
            }
        }

        private void loadImage(string fileName) {
            try {
                int id = images.Count;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                Image img = new Image {
                    Source = bitmap,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    Tag = id
                };
                images.Add(img);
                imagesPos.Add(id, new Point(0,0));
                imageScale.Add(id, new ScaleTransform(1,1));
                imagesCanvas.Children.Add(img);
                img.MouseDown += new MouseButtonEventHandler((sender, e) => onIMGMouseDown(sender, e, img));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void onIMGMouseDown(object sender, MouseButtonEventArgs e, Image image) {
            currentImage = image;
            imageToScale = image;
            clickedPos = e.GetPosition(image);
            //MessageBox.Show("Clicked in: " + clickedPos);
        }
    }
}