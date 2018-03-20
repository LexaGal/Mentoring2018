using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Path = System.IO.Path;

namespace MentoringTasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _tokenSource;

        public MainWindow()
        {
            InitializeComponent();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;             
        }

        private async void LoadPage(object sender, RoutedEventArgs e)
        {
            var downloadableFile = Url.Text;
            var destinationFilePath = Path.GetFullPath($"test_{DateTime.Now.ToFileTime()}.zip");
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            using (var client = new HttpClientDownloadWithProgress(downloadableFile, destinationFilePath, token))
            {
                client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                {
                    if (progressPercentage != null)
                    {
                        ProgressBar.Value = progressPercentage.Value;
                        Percentage.Text = $"{progressPercentage} %";
                    }
                };
                try
                {
                    await client.StartDownload();
                }
                catch (HttpRequestException exception)
                {
                    MessageBox.Show(exception.Message);
                }
                catch (TaskCanceledException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }            
        }

        private void CancelLoad(object sender, RoutedEventArgs e)
        {
            _tokenSource?.Cancel();        
        }
    }
}
