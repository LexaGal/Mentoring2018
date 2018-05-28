using CastleIoC.Interfaces;
using CastleIoC.IoC;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FileProcessingService
{
    public class FileService
    {
        ManualResetEvent _stopWorkEvent;
        List<IDirFilesProcessor> _processors;

        public FileService(string[] inDirs, string outDir, string processedDir, string brokenDir)
        {
            DependencyResolver.Initialize();

            _stopWorkEvent = new ManualResetEvent(false);
            _processors = new List<IDirFilesProcessor>();

            foreach (var inDir in inDirs)
            {
                Directory.CreateDirectory(inDir);
                var dfp = DependencyResolver.For<IDirFilesProcessor>();
                dfp.SetupDirFilesProcessor(inDir, outDir, processedDir, brokenDir, _stopWorkEvent);
                _processors.Add(dfp);
            }

            Directory.CreateDirectory(outDir);
            Directory.CreateDirectory(processedDir);
            Directory.CreateDirectory(brokenDir);
        }

        public void Start()
        {
            foreach (var processor in _processors)
            {
                processor.Start();                
            }
        }

        public void Stop()
        {
            foreach (var processor in _processors)
            {
                processor.Stop();               
            }
            _stopWorkEvent.Set();
        }

    }
}
