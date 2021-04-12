using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PoE.BuffWatcher.Capture;

namespace PoE.BuffWatcher.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;

        private WindowHandler _windowHandler;

        private GameWindowCapturer _gameWindowCapturer;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _timer?.Dispose();
            base.OnClosing(e);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer = new Timer(TimerCallback, null, TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(400));
            string windowName = "Path Of Exile";

            _windowHandler = new WindowHandler(windowName);
            _gameWindowCapturer = new GameWindowCapturer(_windowHandler);

        }

        private void TimerCallback(object? state)
        {
            Dispatcher.Invoke(UpdateDebuff);
        }

        private void UpdateDebuff()
        {
            var image = _gameWindowCapturer.GetBitmapFromGameWindow();
            if (image != null)
            {
                using var imageScanner = new ImageScanner(image);

                var debuffs = imageScanner.FindDebuffs();
                if (debuffs != null)
                {
                    var source = ConvertToImageSource(debuffs);
                    PositionWindow(debuffs);
                    DebuffImage.Source = source;
                    debuffs.Dispose();
                    return;
                }
            }

            // Set to 0 size if we don't have anything to show
            DebuffImage.Width = 0;
            DebuffImage.Height = 0;
        }

        private void PositionWindow(Image debuffs)
        {
            var rect = _windowHandler.GetWindowRect();

            this.Top = (rect.Bottom - rect.Top) * 0.55;
            this.Left = (rect.Right - rect.Left) / 2 - (debuffs.Width / 2);

            this.Width = debuffs.Width;
            this.Height = debuffs.Height;
            DebuffImage.Width = debuffs.Width;
            DebuffImage.Height = debuffs.Height;
        }

        private ImageSource ConvertToImageSource(Image debuffs)
        {
            using var memoryStream = new MemoryStream();
            debuffs.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
