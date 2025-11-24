#if UNIT_DI
#nullable enable
namespace UniT.Lifecycle
{
    using System.Collections.Generic;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class DILifecycleManager : LifecycleManager
    {
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
            ILoggerManager                   loggerManager
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
            resumeListeners,
            loggerManager
        )
        {
        }
    }
}
#endif