using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FileProcessingService
{
    public class FileService
    {
        ManualResetEvent _stopWorkEvent;
        List<DirFilesProcessor> _processors;

        public FileService(string[] inDirs, string outDir, string processedDir, string brokenDir)
        {
            _stopWorkEvent = new ManualResetEvent(false);
            _processors = new List<DirFilesProcessor>();

            foreach (var inDir in inDirs)
            {
                Directory.CreateDirectory(inDir);
                _processors.Add(new DirFilesProcessor(inDir, outDir, processedDir, brokenDir, _stopWorkEvent));
            }

            Directory.CreateDirectory(outDir);
            Directory.CreateDirectory(processedDir);
            Directory.CreateDirectory(brokenDir);
        }

        public void Start()
        {
            foreach (var processor in _processors)
            {
                processor.WorkThread.Start();
                processor.Watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            foreach (var processor in _processors)
            {
                processor.Watcher.EnableRaisingEvents = false;
                processor.WorkThread.Join();
            }
            _stopWorkEvent.Set();
        }

    }
}
