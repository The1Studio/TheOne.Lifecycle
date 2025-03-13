﻿#if UNIT_ZENJECT
#nullable enable
namespace UniT.Lifecycle
{
    using System.Collections.Generic;
    using UnityEngine.Scripting;
    using Zenject;

    public sealed class ZenjectLifecycleManager : LifecycleManager
    {
        [Preserve]
        public ZenjectLifecycleManager(
            [InjectLocal] IEnumerable<IEarlyLoadable>      earlyLoadableServices,
            [InjectLocal] IEnumerable<IAsyncEarlyLoadable> asyncEarlyLoadableServices,
            [InjectLocal] IEnumerable<ILoadable>           loadableServices,
            [InjectLocal] IEnumerable<IAsyncLoadable>      asyncLoadableServices,
            [InjectLocal] IEnumerable<ILateLoadable>       lateLoadableServices,
            [InjectLocal] IEnumerable<IAsyncLateLoadable>  asyncLateLoadableServices,
            [InjectLocal] IEnumerable<IUpdatable>          updatableServices,
            [InjectLocal] IEnumerable<ILateUpdatable>      lateUpdatableServices,
            [InjectLocal] IEnumerable<IFixedUpdatable>     fixedUpdatableServices,
            [InjectLocal] IEnumerable<IFocusGainListener>  focusGainListeners,
            [InjectLocal] IEnumerable<IFocusLostListener>  focusLostListeners,
            [InjectLocal] IEnumerable<IPauseListener>      pauseListeners,
            [InjectLocal] IEnumerable<IResumeListener>     resumeListeners
        ) : base(
            earlyLoadableServices,
            asyncEarlyLoadableServices,
            loadableServices,
            asyncLoadableServices,
            lateLoadableServices,
            asyncLateLoadableServices,
            updatableServices,
            lateUpdatableServices,
            fixedUpdatableServices,
            focusGainListeners,
            focusLostListeners,
            pauseListeners,
            resumeListeners
        )
        {
        }
    }
}
#endif