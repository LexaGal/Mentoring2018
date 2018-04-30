using System;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ServerService
{
    public class SaverService
    {
        ManualResetEvent _stopWorkEvent;
        string _outDir;
        CloudBlobContainer _cloudBlobContainer;

        public FileSystemWatcher Watcher { get; }
        public Thread WorkThread { get; }

        public SaverService(string outDir, string settingsDir)
        {
            _outDir = outDir;
            _stopWorkEvent = new ManualResetEvent(false);
            Directory.CreateDirectory(outDir);

            Watcher = new FileSystemWatcher(settingsDir);
            Watcher.Changed += Watcher_Changed;
            WorkThread = new Thread(ProcessFiles);

            CloudStorageAccount storageAccount;
            var storageConnectionString = ConfigurationManager.AppSettings["Storage"];
            if (!CloudStorageAccount.TryParse(storageConnectionString, out storageAccount)) return;
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            _cloudBlobContainer =
                cloudBlobClient.GetContainerReference(ConfigurationManager.AppSettings["Container"]);
        }

        private void ProcessFiles(object obj)
        {
            do
            {
                if (_stopWorkEvent.WaitOne(TimeSpan.Zero)) return;
                RecieveFiles().GetAwaiter().GetResult();
            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent}, 3000) != 0);
        }

        private async Task RecieveFiles()
        {
            var queueClient =
                QueueClient.Create(ConfigurationManager.AppSettings["Queue"], ReceiveMode.ReceiveAndDelete);
            var message = await queueClient.ReceiveAsync();
            var id = message.GetBody<string>();
            await queueClient.CloseAsync();

            var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(id);
            await cloudBlockBlob.DownloadToFileAsync($@"{_outDir}\{id}.pdf", FileMode.Create);
            await cloudBlockBlob.DeleteAsync();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //var pdf = PdfDocument.nderPdfDoc//pdf.Save(ms, false)
        }

        public void Start()
        {
            WorkThread.Start();
            Watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            Watcher.EnableRaisingEvents = false;
            WorkThread.Join();
        }

    }
}
