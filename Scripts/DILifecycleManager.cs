#if UNIT_DI
#nullable enable
namespace UniT.Lifecycle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    public sealed class DILifecycleManager : LifecycleManager
    {
        private readonly IReadOnlyList<IDisposable> disposableServices;

        [Preserve]
        public DILifecycleManager(
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
            IEnumerable<IResumeListener>     resumeListeners,
            IEnumerable<IDisposable>         disposableServices
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
            this.disposableServices = disposableServices.ToArray();
        }

        public new void Unload()
        {
            this.disposableServices.ForEach(service => service.Dispose());
            base.Unload();
        }
    }
}
#endif