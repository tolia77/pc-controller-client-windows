using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using SocketApp;

namespace PcControllerClientWPF
{
    public partial class MainWindow : Window
    {
        private bool streamScreen = false;
        private SocketRequests socketRequests;
        public MainWindow()
        {
            InitializeComponent();
            MoveMouseButton.IsEnabled = false;
            OpenLinkButton.IsEnabled = false;
            CmdExecuteButton.IsEnabled = false;
            ShutdownPcButton.IsEnabled = false;
            RestartPcButton.IsEnabled = false;
            GetScreenshotButton.IsEnabled = false;
            StreamScreenButton.IsEnabled = false;
            StopStreamButton.IsEnabled = false;
        }
        public void ConnectClicked(object sender, EventArgs e)
        {
            try
            {
                socketRequests = new(IpTextBox.Text);
                MoveMouseButton.IsEnabled = true;
                OpenLinkButton.IsEnabled = true;
                CmdExecuteButton.IsEnabled = true;
                ShutdownPcButton.IsEnabled = true;
                RestartPcButton.IsEnabled = true;
                GetScreenshotButton.IsEnabled = true;
                StreamScreenButton.IsEnabled = true;
            }
            catch (Exception)
            {
                string messageBoxText = $"Error connecting to {IpTextBox.Text}";
                string caption = "Connection Error";
                MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }
        public void MoveMouseClicked(object sender, EventArgs e)
        {
            if
            (
            !string.IsNullOrEmpty(RepeatsTextBox.Text) &&
            !string.IsNullOrEmpty(IntervalTextBox.Text) &&
            long.TryParse(RepeatsTextBox.Text, out long temp) && temp > 0 &&
            long.TryParse(IntervalTextBox.Text, out temp) && temp > 0
            )
            {
                try
                {
                    string result = socketRequests.MoveMouseRequest(RepeatsTextBox.Text, IntervalTextBox.Text);
                    if (result == "ERROR")
                    {
                        string messageBoxText = "Trouble on server";
                        string caption = "Error";
                        MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
        }
        public void OpenLinkClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LinkTextBox.Text))
            {
                try
                {
                    string result = socketRequests.OpenLinkRequest(LinkTextBox.Text);
                    if (result == "ERROR")
                    {
                        string messageBoxText = $"Trouble on server";
                        string caption = "Error";
                        MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
        }
        public void CmdExecuteClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CommandTextBox.Text))
            {
                try
                {
                    string result = socketRequests.CmdExecuteRequest(CommandTextBox.Text);
                    if (result == "ERROR")
                    {
                        string messageBoxText = $"Trouble on server";
                        string caption = "Error";
                        MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
        }
        public void ShutdownPcClicked(object sender, EventArgs e)
        {
            try
            {
                socketRequests.ShutdownPcRequest();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }
        public void RestartPcClicked(object sender, EventArgs e)
        {
            try
            {
                socketRequests.RestartPcRequest();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }
        public void GetScreenshotClicked(object sender, EventArgs e)
        {
            try
            {
                using Stream stream = new MemoryStream(socketRequests.GetScreenshotRequest());
                BitmapImage image = new();
                stream.Position = 0;
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                ScreenshotImage.Source = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }
        public void StreamScreenClicked(object sender, EventArgs e)
        {
            StopStreamButton.IsEnabled = true;
            StreamScreenButton.IsEnabled = false;
            Thread thread = new(() => StreamScreen());
            thread.Start();
        }

        public void StopStreamClicked(object sender, EventArgs e)
        {
            streamScreen = false;
            StopStreamButton.IsEnabled = false;
            StreamScreenButton.IsEnabled = true;
        }
        public void StreamScreen()
        {
            streamScreen = true;
            while (streamScreen)
            {
                try
                {
                    using Stream stream = new MemoryStream(socketRequests.GetScreenshotRequest());
                    BitmapImage image = new();
                    stream.Position = 0;
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze();
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        ScreenshotImage.Source = image;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
        }
    }
}
