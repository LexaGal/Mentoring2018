using Castle.DynamicProxy;
using CastleIoC.Serializing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CastleIoC.Interceptors
{
    public class LoggingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var path = Path.Combine(currentDir, "interceptions.json");
            var arr = JsonConvert.DeserializeObject<List<LoggingInfo>>(File.ReadAllText(path));
            var log = new LoggingInfo(invocation);
            arr.Add(log);
            invocation.Proceed();
            log = new LoggingInfo(invocation);
            arr.Add(log);
            File.WriteAllText(path, string.Join(",", JsonConvert.SerializeObject(arr)));
        }
    }
}
