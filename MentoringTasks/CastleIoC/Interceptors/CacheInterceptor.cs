using System.Runtime.Caching;
using Castle.DynamicProxy;

namespace CastleIoC.Interceptors
{
	public class CacheInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			var key = GetCacheKey(invocation.Arguments);
			var value = MemoryCache.Default.Get(key);

			if (value == null)
			{
				invocation.Proceed();
				value = invocation.ReturnValue;
                if (value == null) return;
                MemoryCache.Default.Set(key, value, new CacheItemPolicy());
                return;
			}
			invocation.ReturnValue = value;			
		}

		string GetCacheKey(object[] arguments)
		{
			return string.Join(";", arguments);
		}
	}
}
