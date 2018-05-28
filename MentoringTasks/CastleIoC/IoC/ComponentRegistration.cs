using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using CastleIoC.Interceptors;
using CastleIoC.Interfaces;
using FileProcessingService;

namespace CastleIoC.IoC
{
    public class ComponentRegistration : IRegistration
    {
        public void Register(IKernelInternal kernel)
        {
            kernel.Register(Component.For<LoggingInterceptor>());

            kernel.Register(Component.For<CacheInterceptor>());

            kernel.Register(
                Component.For<IDirFilesProcessor>()
                         .ImplementedBy<DirFilesProcessor>()
                         .Interceptors(InterceptorReference.ForType<LoggingInterceptor>(), 
                         InterceptorReference.ForType<CacheInterceptor>()).Anywhere);          
        }
    }
}
