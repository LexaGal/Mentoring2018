using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ServerService
{
    public class SaverService
    {
        ManualResetEvent _stopWorkEvent;
        string _outDir;

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
        }

        private void ProcessFiles(object obj)
        {
            do
            {
                if (_stopWorkEvent.WaitOne(TimeSpan.Zero)) return;

            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopWorkEvent}, 3000) != 0);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //AutoResetEvent _fileEditEvent;
            //_fileEditEvent = new AutoResetEvent(false);
            //, _fileEditEvent_fileEditEvent.Set();           
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
