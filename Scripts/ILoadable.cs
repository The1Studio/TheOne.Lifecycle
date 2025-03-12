#nullable enable
namespace UniT.Lifecycle
{
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System;
    using System.Collections;
    #endif

    public interface IEarlyLoadable
    {
        public void Load();
    }

    public interface ILoadable
    {
        public void Load();
    }

    public interface ILateLoadable
    {
        public void Load();
    }

    #if UNIT_UNITASK
    public interface IAsyncEarlyLoadable
    {
        public UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncLoadable
    {
        public UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncLateLoadable
    {
        public UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }
    #else
    public interface IAsyncEarlyLoadable
    {
        public IEnumerator LoadAsync(Action? callback = null, IProgress<float>? progress = null);
    }

    public interface IAsyncLoadable
    {
        public IEnumerator LoadAsync(Action? callback = null, IProgress<float>? progress = null);
    }

    public interface IAsyncLateLoadable
    {
        public IEnumerator LoadAsync(Action? callback = null, IProgress<float>? progress = null);
    }
    #endif
}