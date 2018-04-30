using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FileProcessingService
{
    public class SenderService
    {
        ManualResetEvent _stopWorkEvent;
        List<DirFilesProcessor> _processors;

        public SenderService(string[] inDirs)
        {
            _stopWorkEvent = new ManualResetEvent(false);
            _processors = new List<DirFilesProcessor>();

            foreach (var inDir in inDirs)
            {
                Directory.CreateDirectory(inDir);
                _processors.Add(new DirFilesProcessor(inDir, _stopWorkEvent));
            }
        }

        public void Start()
        {
            foreach (var processor in _processors)
            {
                processor.WorkThread.Start();
                processor.SendInfoThread.Start();
                processor.RecieveInfoThread.Start();
                processor.Watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            foreach (var processor in _processors)
            {
                processor.Watcher.EnableRaisingEvents = false;
                processor.WorkThread.Join();
                processor.SendInfoThread.Join();
                processor.RecieveInfoThread.Join();
            }
            _stopWorkEvent.Set();
        }
    }

    public enum DirServiceState
    {
        Waiting,
        Processing
    } 
}
