using System;

namespace Scope.Core
{
    public interface IService { }

    public interface IServiceUpdater<T>
    {
        void SetService(T newService);
    }

    public interface IServiceProvider<T> : IService
    {
        T CurrentService { get; }
        event EventHandler<ServiceProviderEventArgs<T>> OnServiceChanged;
    }

    public interface IUpdateableServiceProvider<T> : IServiceUpdater<T>, IServiceProvider<T> { }

    public class ServiceProviderEventArgs<T> : EventArgs
    {
        public readonly T NewService;
        public readonly T OldService;

        public ServiceProviderEventArgs(T newService, T oldService)
        {
            this.NewService = newService;
            this.OldService = oldService;
        }
    }

    public interface IServiceImplementer
    {
        void CleanUp();
    }

    public abstract class ServiceBase : IServiceImplementer
    {
        public virtual void CleanUp() { }
    }
}
