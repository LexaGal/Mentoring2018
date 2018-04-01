using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MentoringTasks
{
    class Program
    {
        static string _exePath = @"C:\Program Files (x86)\Google\Chrome\Application\";
        static string _args = $"--enable-logging --headless --disable-gpu --print-to-pdf=\"{_exePath}temp.pdf\"";
        static string _url = "https://www.chromestatus.com/features#browsers.chrome.status%3A%22In%20development%22";
        static string _test = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\test.pdf";
        static List<string> _errors = new List<string>();

        static void Main()
        {
            Console.WriteLine("Printing...");
            var task = RunSeleniumAsync();
            //RunProcessAsync($"{_exePath}chrome.exe", $"{_args} {_url}");              

            var result = task.GetAwaiter().GetResult();
            //_errors.ToList().ForEach(Console.WriteLine);

            if (result == null)
            {
                task.Exception?.InnerExceptions.SelectMany(e => e.Message).ToList().ForEach(Console.WriteLine);
                Console.ReadKey();
                return;
            }
            File.WriteAllBytes(_test, result.ToArray());
            Console.WriteLine("File was saved as test.pdf");
            Console.ReadKey();
        }

        public static Task<MemoryStream> RunProcessAsync(string processPath, string args)
        {
            var tcs = new TaskCompletionSource<MemoryStream>();

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(processPath, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.ErrorDataReceived += (sender, errArgs) =>
            {
                _errors.Add(errArgs.Data);

                // ignore UI manifest errors here:
                if (errArgs.Data != null && errArgs.Data.Contains("FILE_ERROR"))
                {
                    tcs.SetResult(null);
                }
            };

            process.Exited += (sender, exitArgs) =>
            {
                if (process.ExitCode != 0)
                {
                    var errorMessage = process.StandardError.ReadToEnd();
                    tcs.SetException(new InvalidOperationException(
                        "The process did not exit correctly. The corresponding error message is: " + errorMessage));
                    tcs.SetResult(null);
                }
                else
                {
                    using (var sr = new StreamReader(new FileStream($"{_exePath}temp.pdf", FileMode.Open)))
                    {
                        MemoryStream stream = new MemoryStream();
                        sr.BaseStream.CopyTo(stream);
                        tcs.SetResult(stream);
                    }
                    File.Delete($"{_exePath}temp.pdf");
                }
                process.Dispose();
            };

            process.Start();
            process.BeginErrorReadLine();
            return tcs.Task;
        }

        public static Task<MemoryStream> RunSeleniumAsync()
        {
            var tcs = new TaskCompletionSource<MemoryStream>();
            var pageConverter = new PageConverter();
            var stream = pageConverter.GetPdf(_url);
            tcs.SetResult(stream);
            return tcs.Task;
        }

    }
}
