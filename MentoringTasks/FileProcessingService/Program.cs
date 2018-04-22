using NLog;
using NLog.Config;
using NLog.Targets;
using System.Diagnostics;
using System.IO;
using Topshelf;

namespace FileProcessingService
{
    public class Program
    {
        static void Main()
        {
            var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            if (currentDir == null)
            {
                return;
            }

            var inDirs = new[]
            {
                Path.Combine(currentDir, "in1"),
                Path.Combine(currentDir, "in2")
            };
            var outDir = Path.Combine(currentDir, "out");
            var processedDir = Path.Combine(currentDir, "processed");
            var brokenDir = Path.Combine(currentDir, "broken");
            
            var conf = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                Name = "Default",
                FileName = Path.Combine(currentDir, "log.txt"),
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };
            conf.AddTarget(fileTarget);
            conf.AddRuleForAllLevels(fileTarget);

            var logFactory = new LogFactory(conf);

            HostFactory.Run(
                hostConf => hostConf.Service<FileService>(
                    s =>
                    {
                        s.ConstructUsing(() => new FileService(inDirs, outDir, processedDir, brokenDir));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }
                ).UseNLog(logFactory));
        }
    }
}
