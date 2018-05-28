using System;
using Castle.DynamicProxy;

namespace CastleIoC.Serializing
{
    class LoggingInfo
    {
        public LoggingInfo() { }

        public LoggingInfo(IInvocation invocation)
        {
            MethodName = invocation.Method.Name;
            Time = DateTime.Now.ToLongTimeString();
            Arguments = string.Join(",", invocation.Arguments);
            Return = invocation.ReturnValue;
        }

        public string MethodName;
        public string Time;
        public string Arguments;
        public object Return;                     
    }
}
