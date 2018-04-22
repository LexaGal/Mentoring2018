using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ZXing;

namespace FileProcessingService
{
    public class DirFilesProcessor
    {
        public DirFilesProcessor(string inDir, string outDir, string processedDir, string brokenDir,
            ManualResetEvent stopWorkEvent)
        {
            _newFileEvent = new AutoResetEvent(false);
            _files = new List<string>();
            _document = new Document();
            _document.AddSection();

            _inDir = inDir;
            _outDir = outDir;
            _processedDir = processedDir;
            _brokenDir = brokenDir;

            _stopWorkEvent = stopWorkEvent;

            Watcher = new FileSystemWatcher(inDir);
            Watcher.Created += Watcher_Created;
            WorkThread = new Thread(ProcessFiles);
        }

        string _inDir;
        string _outDir;
        string _processedDir;
        string _brokenDir;

        ManualResetEvent _stopWorkEvent;
        
        AutoResetEvent _newFileEvent;
        List<string> _files;
        Document _document; 
         
        int _tryOpenFileAttempts = 3;
        int _tryOpenFileDelay = 3000;
        string _pattern = "IMG_[0-9]+.(PNG|JPEG|BMP)";

        public FileSystemWatcher Watcher { get; }
        public Thread WorkThread { get; }

        private void ProcessFiles(object obj)
        {
            do
            {
                var destDir = _processedDir;

                foreach (var file in Directory.EnumerateFiles(_inDir).ToList())
                {
                    if (_stopWorkEvent.WaitOne(TimeSpan.Zero)) return;
                    var fileName = Path.GetFileName(file);
                    if (fileName == null) continue;
                    if (!Regex.IsMatch(fileName.ToUpper(), _pattern)) continue;

                    if (TryOpenFile(file, _tryOpenFileAttempts))
                    {
                        try
                        {
                            if (FileIsBarcode(file))
                            {
                                RenderPdfDocument();
                                MoveFiles(_files, destDir);
                                _files.Clear();
                                destDir = _processedDir;
                                if (TryOpenFile(file, _tryOpenFileAttempts)) File.Delete(file);
                            }
                            else
                            {
                                if (!_files.Contains(file))
                                {
                                    _files.Add(file);
                                    AddFileToPdfDocument(file);
                                }
                            }
                        }
                        catch (OutOfMemoryException)
                        {
                            if (!_files.Contains(file)) _files.Add(file);
                            destDir = _brokenDir;
                        }
                    }
                }
            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent, _newFileEvent}, 3000) != 0);
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            _newFileEvent.Set();
        }

        private void MoveFiles(IEnumerable<string> _files, string destDir)
        {
            var fileTime = DateTime.Now.ToFileTime().ToString();
            foreach (var file in _files)
            {
                if (!TryOpenFile(file, _tryOpenFileAttempts)) continue;
                var name = Path.GetFileName(file);
                var dir = Path.Combine(destDir, fileTime);
                Directory.CreateDirectory(dir);
                File.Move(file, Path.Combine(dir, name));
            }
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
            ((Section) _document.Sections.First).AddPageBreak();
            ((Section) _document.Sections.First).AddImage(file);
        }

        private void RenderPdfDocument()
        {
            var render = new PdfDocumentRenderer {Document = _document};
            render.RenderDocument();
            render.Save($@"{_outDir}\{DateTime.Now.ToFileTime()}.pdf");
            _document = new Document();
            _document.AddSection();
        }

        private bool FileIsBarcode(string file)
        {
            var reader = new BarcodeReader {AutoRotate = true};
            var bmp = (Bitmap) Image.FromFile(file);
            var result = reader.Decode(bmp);
            return result?.Text.ToUpper() == "SEPARATOR";
        }
    }
}