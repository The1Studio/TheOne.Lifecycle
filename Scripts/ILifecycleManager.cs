#nullable enable
namespace TheOne.Lifecycle
{
    using System;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface ILifecycleManager : IDisposable
    {
        #if THEONE_UNITASK
        public UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator LoadAsync(Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}