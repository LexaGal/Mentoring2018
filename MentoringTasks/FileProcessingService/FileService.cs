using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ZXing;
using ZXing.QrCode;

namespace FileProcessingService
{
    public class FileService
    {
        FileSystemWatcher _watcher;
        Thread _workThread;
        ManualResetEvent _stopWorkEvent;
        AutoResetEvent _newFileEvent;

        string _inDir;
        string _outDir;

        int _tryOpenFileAttempts = 3;
        int _tryOpenFileDelay = 5000;
        string _pattern = "IMG_[0-9]+.(PNG|JPEG|BMP)";

        Document _document;

        public FileService(string inDir, string outDir)
        {
            _inDir = inDir;
            _outDir = outDir;

            if (!Directory.Exists(inDir))
                Directory.CreateDirectory(inDir);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            _watcher = new FileSystemWatcher(inDir);
            _watcher.Created += Watcher_Created;

            _workThread = new Thread(ProcessFiles);
            _stopWorkEvent = new ManualResetEvent(false);
            _newFileEvent = new AutoResetEvent(false);

            _document = new Document();
            _document.AddSection();
        }

        private void ProcessFiles(object obj)
        {
            do
            {
                foreach (var file in Directory.EnumerateFiles(_inDir))
                {
                    if (_stopWorkEvent.WaitOne(TimeSpan.Zero))
                        return;

                    var fileName = Path.GetFileName(file);

                    if (fileName == null) continue;
                    if (!Regex.IsMatch(fileName.ToUpper(), _pattern)) continue;

                    if (TryOpenFile(file, _tryOpenFileAttempts))
                    {
                        if (FileIsBarcode(file))
                        {
                            RenderPdfDocument();
                        }
                        else
                        {
                            AddFileToPdfDocument(file);
                        }
                        File.Delete(file);
                    }
                }

            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent, _newFileEvent}, 1000) != 0);
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            _newFileEvent.Set();
        }

        public void Start()
        {
            _workThread.Start();
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _stopWorkEvent.Set();
            _workThread.Join();
        }

        private bool TryOpenFile(string fileName, int attempts)
        {
            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(_tryOpenFileDelay);
                }
            }

            return false;
        }

        private void AddFileToPdfDocument(string file)
        {
            var img = ((Section)_document.Sections.First).AddImage(file);
            img.Height = _document.DefaultPageSetup.PageHeight;
            img.Width = _document.DefaultPageSetup.PageWidth;
        }

        private void RenderPdfDocument()
        {
            var render = new PdfDocumentRenderer {Document = _document};
            render.RenderDocument();
            render.Save($@"{_outDir}\file{DateTime.Now.ToFileTime()}.pdf");
            _document = new Document();
            _document.AddSection();
        }
        
        private bool FileIsBarcode(string file)
        {
            var reader = new BarcodeReader { AutoRotate = true}; 
            var bmp = (Bitmap)Image.FromFile(file);
            var result = reader.Decode(bmp);
            return result?.Text.ToUpper() == "SEPARATOR";
        }
    }
}
