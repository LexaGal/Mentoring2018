using System;
using System.Collections.Generic;
using System.Threading;

namespace CastleIoC.Interfaces
{
    public interface IDirFilesProcessor
    {
        bool SetupDirFilesProcessor(string inDir, string outDir, string processedDir, string brokenDir,
            ManualResetEvent stopWorkEvent);
        string Start();
        DateTime Stop();
    }
}
