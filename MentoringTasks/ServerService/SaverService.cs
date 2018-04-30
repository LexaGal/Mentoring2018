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
        int _waitDelay = 1000;

        public FileSystemWatcher Watcher { get; }
        public Thread WorkThread { get; }
        public Thread SendInfoThread { get; }
        public Thread RecieveInfoThread { get; }

        public SaverService(string outDir, string settingsDir)
        {
            _outDir = outDir;
            _stopWorkEvent = new ManualResetEvent(false);
            Directory.CreateDirectory(outDir);

            Watcher = new FileSystemWatcher(settingsDir);
            Watcher.Changed += Watcher_Changed;
            WorkThread = new Thread(ProcessFiles);
            SendInfoThread = new Thread(SendInfo);
            RecieveInfoThread = new Thread(RecieveInfo);

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
            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent}, _waitDelay) != 0);
        }

        private async Task RecieveFiles()
        {
            var queueClient =
                QueueClient.Create(ConfigurationManager.AppSettings["FilesQueue"], ReceiveMode.ReceiveAndDelete);
            var message = await queueClient.ReceiveAsync(new TimeSpan(1000));
            if (message == null) return;
            var id = message.GetBody<string>();
            await queueClient.CloseAsync();

            var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(id);
            await cloudBlockBlob.DownloadToFileAsync($@"{_outDir}\{id}.pdf", FileMode.Create);
            await cloudBlockBlob.DeleteAsync();
        }

        private void SendInfo(object obj)
        {
            do
            {
                SendState().GetAwaiter().GetResult();
                SendBarcode().GetAwaiter().GetResult();
            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent}, _waitDelay) != 0);
        }

        private void RecieveInfo(object obj)
        {
            do
            {
                RecieveState().GetAwaiter().GetResult();
                RecieveBarcode().GetAwaiter().GetResult();
            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent}, _waitDelay) != 0);
        }

        private async Task SendState()
        {
            var queueClient = QueueClient.Create(ConfigurationManager.AppSettings["SendStatesQueue"]);

            //for 2 services
            var message = new BrokeredMessage("Ask for state");
            await queueClient.SendAsync(message);
            message = new BrokeredMessage("Ask for state");
            await queueClient.SendAsync(message);

            await queueClient.CloseAsync();
            
            Console.WriteLine("Send: Ask for state");
        }

        private async Task SendBarcode()
        {
            var queueClient = QueueClient.Create(ConfigurationManager.AppSettings["UpdateBarcodesQueue"]);
            var newBarcodeText = $"NEW-BARCODE-{DateTime.Now.Second}";
            
            //for 2 services
            var message = new BrokeredMessage(newBarcodeText);
            await queueClient.SendAsync(message);
            message = new BrokeredMessage(newBarcodeText);
            await queueClient.SendAsync(message);

            await queueClient.CloseAsync();

            Console.WriteLine($"Send: {newBarcodeText}");            
        }

        private async Task RecieveState()
        {
            var queueClient =
                QueueClient.Create(ConfigurationManager.AppSettings["StatesQueue"], ReceiveMode.ReceiveAndDelete);
            var message = await queueClient.ReceiveAsync(new TimeSpan(1000));
            if (message == null) return;
            var state = message.GetBody<string>();
            await queueClient.CloseAsync();
            Console.WriteLine($"Recieve: {state}");
        }

        private async Task RecieveBarcode()
        {
            var queueClient =
                QueueClient.Create(ConfigurationManager.AppSettings["BarcodesQueue"], ReceiveMode.ReceiveAndDelete);
            var message = await queueClient.ReceiveAsync(new TimeSpan(1000));
            if (message == null) return;
            var code = message.GetBody<string>();
            var barcodeText = code;
            await queueClient.CloseAsync();
            Console.WriteLine($"Recieve: {barcodeText}");
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
        }

        public void Start()
        {
            WorkThread.Start();
            SendInfoThread.Start();
            RecieveInfoThread.Start();
            Watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            Watcher.EnableRaisingEvents = false;
            WorkThread.Join();
            SendInfoThread.Join();
            RecieveInfoThread.Join();
        }
    }

    public enum DirServiceState
    {
        Waiting,
        Processing
    }
}
