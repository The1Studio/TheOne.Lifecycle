#nullable enable
namespace UniT.Lifecycle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class LifecycleManager : ILifecycleManager
    {
        private readonly IReadOnlyList<IEarlyLoadable>      earlyLoadableServices;
        private readonly IReadOnlyList<IAsyncEarlyLoadable> asyncEarlyLoadableServices;
        private readonly IReadOnlyList<ILoadable>           loadableServices;
        private readonly IReadOnlyList<IAsyncLoadable>      asyncLoadableServices;
        private readonly IReadOnlyList<ILateLoadable>       lateLoadableServices;
        private readonly IReadOnlyList<IAsyncLateLoadable>  asyncLateLoadableServices;

        private readonly IReadOnlyList<IUpdatable>      updatableServices;
        private readonly IReadOnlyList<ILateUpdatable>  lateUpdatableServices;
        private readonly IReadOnlyList<IFixedUpdatable> fixedUpdatableServices;

        private readonly IReadOnlyList<IFocusGainListener> focusGainListeners;
        private readonly IReadOnlyList<IFocusLostListener> focusLostListeners;

        private readonly IReadOnlyList<IPauseListener>  pauseListeners;
        private readonly IReadOnlyList<IResumeListener> resumeListeners;

        protected LifecycleManager(
            IEnumerable<IEarlyLoadable>      earlyLoadableServices,
            IEnumerable<IAsyncEarlyLoadable> asyncEarlyLoadableServices,
            IEnumerable<ILoadable>           loadableServices,
            IEnumerable<IAsyncLoadable>      asyncLoadableServices,
            IEnumerable<ILateLoadable>       lateLoadableServices,
            IEnumerable<IAsyncLateLoadable>  asyncLateLoadableServices,
            IEnumerable<IUpdatable>          updatableServices,
            IEnumerable<ILateUpdatable>      lateUpdatableServices,
            IEnumerable<IFixedUpdatable>     fixedUpdatableServices,
            IEnumerable<IFocusGainListener>  focusGainListeners,
            IEnumerable<IFocusLostListener>  focusLostListeners,
            IEnumerable<IPauseListener>      pauseListeners,
            IEnumerable<IResumeListener>     resumeListeners
        )
        {
            this.earlyLoadableServices      = earlyLoadableServices.ToArray();
            this.asyncEarlyLoadableServices = asyncEarlyLoadableServices.ToArray();
            this.loadableServices           = loadableServices.ToArray();
            this.asyncLoadableServices      = asyncLoadableServices.ToArray();
            this.lateLoadableServices       = lateLoadableServices.ToArray();
            this.asyncLateLoadableServices  = asyncLateLoadableServices.ToArray();

            this.updatableServices      = updatableServices.ToArray();
            this.lateUpdatableServices  = lateUpdatableServices.ToArray();
            this.fixedUpdatableServices = fixedUpdatableServices.ToArray();

            this.focusGainListeners = focusGainListeners.ToArray();
            this.focusLostListeners = focusLostListeners.ToArray();

            this.pauseListeners  = pauseListeners.ToArray();
            this.resumeListeners = resumeListeners.ToArray();
        }

        #if UNIT_UNITASK
        async UniTask ILifecycleManager.LoadAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            if (this.eventListener) return;
            this.eventListener = new GameObject(nameof(LifecycleManager)).AddComponent<EventListener>().DontDestroyOnLoad();

            var subProgresses = progress.CreateSubProgresses(3).ToArray();
            this.earlyLoadableServices.ForEach(service => service.Load());
            await this.asyncEarlyLoadableServices.ForEachAsync(
                (service, progress, cancellationToken) => service.LoadAsync(progress, cancellationToken),
                subProgresses[0],
                cancellationToken
            );
            subProgresses[0]?.Report(1);
            this.loadableServices.ForEach(service => service.Load());
            await this.asyncLoadableServices.ForEachAsync(
                (service, progress, cancellationToken) => service.LoadAsync(progress, cancellationToken),
                subProgresses[1],
                cancellationToken
            );
            subProgresses[1]?.Report(1);
            this.lateLoadableServices.ForEach(service => service.Load());
            await this.asyncLateLoadableServices.ForEachAsync(
                (service, progress, cancellationToken) => service.LoadAsync(progress, cancellationToken),
                subProgresses[2],
                cancellationToken
            );
            subProgresses[2]?.Report(1);
            this.Load();
        }
        #else
        IEnumerator ILifecycleManager.LoadAsync(Action? callback, IProgress<float>? progress)
        {
            if (this.eventListener) return;
            this.eventListener = new GameObject(nameof(LifecycleManager)).AddComponent<EventListener>().DontDestroyOnLoad();

            var subProgresses = progress.CreateSubProgresses(3).ToArray();
            this.earlyLoadableServices.ForEach(service => service.Load());
            yield return this.asyncEarlyLoadableServices.ForEachAsync(
                (service, progress) => service.LoadAsync(progress: progress),
                progress: subProgresses[0]
            );
            subProgresses[0]?.Report(1);
            this.loadableServices.ForEach(service => service.Load());
            yield return this.asyncLoadableServices.ForEachAsync(
                (service, progress) => service.LoadAsync(progress: progress),
                progress: subProgresses[1]
            );
            subProgresses[1]?.Report(1);
            this.lateLoadableServices.ForEach(service => service.Load());
            yield return this.asyncLateLoadableServices.ForEachAsync(
                (service, progress) => service.LoadAsync(progress: progress),
                progress: subProgresses[2]
            );
            subProgresses[2]?.Report(1);
            this.Load();
            callback?.Invoke();
        }
        #endif

        private EventListener eventListener = null!;

        private void Load()
        {
            this.updatableServices.ForEach(service => this.eventListener.Updating           += service.Update);
            this.lateUpdatableServices.ForEach(service => this.eventListener.LateUpdating   += service.LateUpdate);
            this.fixedUpdatableServices.ForEach(service => this.eventListener.FixedUpdating += service.FixedUpdate);

            this.focusGainListeners.ForEach(listener => this.eventListener.FocusGain += listener.OnFocusGain);
            this.focusLostListeners.ForEach(listener => this.eventListener.FocusLost += listener.OnFocusLost);

            this.pauseListeners.ForEach(listener => this.eventListener.Paused   += listener.OnPause);
            this.resumeListeners.ForEach(listener => this.eventListener.Resumed += listener.OnResume);
        }

        private void Unload()
        {
            if (!this.eventListener) return;

            this.updatableServices.ForEach(service => this.eventListener.Updating           -= service.Update);
            this.lateUpdatableServices.ForEach(service => this.eventListener.LateUpdating   -= service.LateUpdate);
            this.fixedUpdatableServices.ForEach(service => this.eventListener.FixedUpdating -= service.FixedUpdate);

            this.focusGainListeners.ForEach(listener => this.eventListener.FocusGain -= listener.OnFocusGain);
            this.focusLostListeners.ForEach(listener => this.eventListener.FocusLost -= listener.OnFocusLost);

            this.pauseListeners.ForEach(listener => this.eventListener.Paused   -= listener.OnPause);
            this.resumeListeners.ForEach(listener => this.eventListener.Resumed -= listener.OnResume);

            this.eventListener.gameObject.Destroy();
        }

        void IDisposable.Dispose() => this.Unload();

        ~LifecycleManager() => this.Unload();

        private sealed class EventListener : MonoBehaviour
        {
            public event Action? Updating;
            public event Action? LateUpdating;
            public event Action? FixedUpdating;

            public event Action? FocusGain;
            public event Action? FocusLost;

            public event Action? Paused;
            public event Action? Resumed;

            private void Update()
            {
                this.Updating?.Invoke();
            }

            private void LateUpdate()
            {
                this.LateUpdating?.Invoke();
            }

            private void FixedUpdate()
            {
                this.FixedUpdating?.Invoke();
            }

            private void OnApplicationFocus(bool hasFocus)
            {
                if (hasFocus)
                {
                    this.FocusGain?.Invoke();
                }
                else
                {
                    this.FocusLost?.Invoke();
                }
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                if (pauseStatus)
                {
                    this.Paused?.Invoke();
                }
                else
                {
                    this.Resumed?.Invoke();
                }
            }
        }
    }
}