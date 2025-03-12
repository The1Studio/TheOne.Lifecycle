#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Lifecycle
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Scripting;
    using VContainer.Internal;

    public sealed class VContainerLifecycleManager : LifecycleManager, IDisposable
    {
        [Preserve]
        public VContainerLifecycleManager(
            ContainerLocal<IEnumerable<IEarlyLoadable>>      earlyLoadableServices,
            ContainerLocal<IEnumerable<IAsyncEarlyLoadable>> asyncEarlyLoadableServices,
            ContainerLocal<IEnumerable<ILoadable>>           loadableServices,
            ContainerLocal<IEnumerable<IAsyncLoadable>>      asyncLoadableServices,
            ContainerLocal<IEnumerable<ILateLoadable>>       lateLoadableServices,
            ContainerLocal<IEnumerable<IAsyncLateLoadable>>  asyncLateLoadableServices,
            ContainerLocal<IEnumerable<IUpdatable>>          updatableServices,
            ContainerLocal<IEnumerable<ILateUpdatable>>      lateUpdatableServices,
            ContainerLocal<IEnumerable<IFixedUpdatable>>     fixedUpdatableServices,
            ContainerLocal<IEnumerable<IFocusGainListener>>  focusGainListeners,
            ContainerLocal<IEnumerable<IFocusLostListener>>  focusLostListeners,
            ContainerLocal<IEnumerable<IPauseListener>>      pauseListeners,
            ContainerLocal<IEnumerable<IResumeListener>>     resumeListeners
        ) : base(
            earlyLoadableServices.Value,
            asyncEarlyLoadableServices.Value,
            loadableServices.Value,
            asyncLoadableServices.Value,
            lateLoadableServices.Value,
            asyncLateLoadableServices.Value,
            updatableServices.Value,
            lateUpdatableServices.Value,
            fixedUpdatableServices.Value,
            focusGainListeners.Value,
            focusLostListeners.Value,
            pauseListeners.Value,
            resumeListeners.Value
        )
        {
        }

        void IDisposable.Dispose() => this.Unload();
    }
}
#endif