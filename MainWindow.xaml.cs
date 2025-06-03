using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Windows.Shapes;


namespace ImAged
{
    public partial class MainWindow : Window
    {
        private BitmapSource? originalImage;
        private double currentRotation = 0;
        private ScaleTransform flipTransform = new ScaleTransform(1, 1);
        private bool isGrayscale = false;
        private bool isSepia = false;
        private bool isInverted = false;
        private bool isResizing = false;
        private Point lastMousePosition;
        private ResizeDirection resizeDirection;

        private Point initialMousePosition;
        private Size initialWindowSize;
        private Point initialWindowPosition;

        private enum ResizeDirection
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += Window_MouseMove;
            this.MouseUp += Window_MouseUp;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp;*.gif)|*.png;*.jpeg;*.jpg;*.bmp;*.gif|All files (*.*)|*.*",
                Title = "Select an image file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Update the selected file text
                    SelectedFileText.Text = System.IO.Path.GetFileName(openFileDialog.FileName);

                    // Load and display the image
                    originalImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    MainImage.Source = originalImage;
                    NoImageText.Visibility = Visibility.Collapsed;

                    // Reset transformations
                    ResetImage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage.Source == null)
            {
                MessageBox.Show("No image to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
                Title = "Save Image As",
                DefaultExt = "png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmapSource = (BitmapSource)MainImage.Source;
                    var encoder = GetEncoder(saveFileDialog.FilterIndex);
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                    using (var stream = File.Create(saveFileDialog.FileName))
                    {
                        encoder.Save(stream);
                    }

                    MessageBox.Show("Image saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private BitmapEncoder GetEncoder(int filterIndex)
        {
            switch (filterIndex)
            {
                case 1:
                    return new PngBitmapEncoder();
                case 2:
                    return new JpegBitmapEncoder();
                case 3:
                    return new BmpBitmapEncoder();
                default:
                    return new PngBitmapEncoder();
            }
        }

        private void RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                currentRotation -= 90;
                ApplyTransformations();
            }
        }

        private void RotateRight_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                currentRotation += 90;
                ApplyTransformations();
            }
        }

        private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                flipTransform.ScaleX *= -1;
                ApplyTransformations();
            }
        }

        private void FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                flipTransform.ScaleY *= -1;
                ApplyTransformations();
            }
        }

        private void Grayscale_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                isGrayscale = !isGrayscale;
                isSepia = false;
                isInverted = false;
                ApplyTransformations();
            }
        }

        private void Sepia_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                isSepia = !isSepia;
                isGrayscale = false;
                isInverted = false;
                ApplyTransformations();
            }
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                isInverted = !isInverted;
                ApplyTransformations();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                ResetImage();
            }
        }

        private void ResetImage()
        {
            currentRotation = 0;
            flipTransform.ScaleX = 1;
            flipTransform.ScaleY = 1;
            isGrayscale = false;
            isSepia = false;
            isInverted = false;
            BrightnessSlider.Value = 0;
            ContrastSlider.Value = 0;
            SaturationSlider.Value = 0;
            ApplyTransformations();
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (originalImage != null)
            {
                ApplyTransformations();
            }
        }

        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (originalImage != null)
            {
                ApplyTransformations();
            }
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (originalImage != null)
            {
                ApplyTransformations();
            }
        }

        private void ApplyTransformations()
        {
            if (originalImage == null) return;

            var transformGroup = new TransformGroup();
            
            // Apply rotation
            transformGroup.Children.Add(new RotateTransform(currentRotation));
            
            // Apply flip
            transformGroup.Children.Add(flipTransform);

            // Create transformed image
            var transformedImage = new TransformedBitmap(originalImage, transformGroup);

            // Apply brightness, contrast, and saturation
            var brightness = BrightnessSlider.Value / 100.0;
            var contrast = ContrastSlider.Value / 100.0;
            var saturation = SaturationSlider.Value / 100.0;

            var adjustedImage = new FormatConvertedBitmap(transformedImage, PixelFormats.Bgra32, null, 0);
            var pixels = new byte[adjustedImage.PixelWidth * adjustedImage.PixelHeight * 4];
            adjustedImage.CopyPixels(pixels, adjustedImage.PixelWidth * 4, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                double b = pixels[i];
                double g = pixels[i + 1];
                double r = pixels[i + 2];

                // Apply brightness
                b *= (1 + brightness);
                g *= (1 + brightness);
                r *= (1 + brightness);

                // Apply contrast
                var factor = (259.0 * (contrast + 1)) / (255.0 * (1 - contrast));
                b = factor * (b - 128) + 128;
                g = factor * (g - 128) + 128;
                r = factor * (r - 128) + 128;

                // Apply saturation
                var gray = 0.2989 * r + 0.5870 * g + 0.1140 * b;
                r = gray + (r - gray) * (1 + saturation);
                g = gray + (g - gray) * (1 + saturation);
                b = gray + (b - gray) * (1 + saturation);

                // Apply filters
                if (isGrayscale)
                {
                    var grayValue = 0.2989 * r + 0.5870 * g + 0.1140 * b;
                    r = g = b = grayValue;
                }
                else if (isSepia)
                {
                    var tr = 0.393 * r + 0.769 * g + 0.189 * b;
                    var tg = 0.349 * r + 0.686 * g + 0.168 * b;
                    var tb = 0.272 * r + 0.534 * g + 0.131 * b;
                    r = Math.Min(255, tr);
                    g = Math.Min(255, tg);
                    b = Math.Min(255, tb);
                }

                if (isInverted)
                {
                    r = 255 - r;
                    g = 255 - g;
                    b = 255 - b;
                }

                // Clamp values
                pixels[i] = (byte)Math.Max(0, Math.Min(255, b));
                pixels[i + 1] = (byte)Math.Max(0, Math.Min(255, g));
                pixels[i + 2] = (byte)Math.Max(0, Math.Min(255, r));
            }

            var finalImage = BitmapSource.Create(
                adjustedImage.PixelWidth,
                adjustedImage.PixelHeight,
                adjustedImage.DpiX,
                adjustedImage.DpiY,
                PixelFormats.Bgra32,
                null,
                pixels,
                adjustedImage.PixelWidth * 4);

            MainImage.Source = finalImage;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                Point currentPosition = e.GetPosition(this);
                double dx = currentPosition.X - initialMousePosition.X;
                double dy = currentPosition.Y - initialMousePosition.Y;

                double newWidth = initialWindowSize.Width;
                double newHeight = initialWindowSize.Height;
                double newLeft = initialWindowPosition.X;
                double newTop = initialWindowPosition.Y;

                double minWidth = 400;
                double minHeight = 300;

                switch (resizeDirection)
                {
                    case ResizeDirection.BottomLeft:
                        newWidth = initialWindowSize.Width - dx;
                        newLeft = initialWindowPosition.X + dx;
                        newHeight = initialWindowSize.Height + dy;
                        // No change to newTop for BottomLeft resize
                        break;
                    // Removed cases for Left, Right, Top, Bottom, TopLeft, TopRight, BottomRight
                }

                if (resizeDirection == ResizeDirection.BottomLeft) // Only apply changes for BottomLeft
                {
                    if (newWidth > minWidth)
                    {
                        Width = newWidth;
                        Left = newLeft; // Apply newLeft only for horizontal changes
                    }

                    if (newHeight > minHeight)
                    {
                        Height = newHeight;
                        // No change to Top for BottomLeft resize
                    }
                }

                // Update lastMousePosition - still useful for potential future refinements or debugging
                lastMousePosition = currentPosition;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && isResizing)
            {
                isResizing = false;
                Mouse.Capture(null);
            }
        }

        private void ResizeGrip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var grip = sender as FrameworkElement;
                if (grip == null) return;

                // Only initiate resizing if the BottomLeftGrip is clicked
                if (grip.Name == "BottomLeftGrip")
                {
                    isResizing = true;
                    initialMousePosition = e.GetPosition(this);
                    initialWindowSize = new Size(ActualWidth, ActualHeight);
                    initialWindowPosition = new Point(Left, Top);
                    Mouse.Capture(this);
                    resizeDirection = ResizeDirection.BottomLeft;
                }
                // Removed logic for other grips
            }
        }
    }
} 