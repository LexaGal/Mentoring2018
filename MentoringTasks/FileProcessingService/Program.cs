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

            var inDir = Path.Combine(currentDir, "in");
            var outDir = Path.Combine(currentDir, "out");
            
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
                        s.ConstructUsing(() => new FileService(inDir, outDir));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }
                ).UseNLog(logFactory));
        }
    }
}
