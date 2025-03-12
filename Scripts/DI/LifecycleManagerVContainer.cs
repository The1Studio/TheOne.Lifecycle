#if THEONE_VCONTAINER
#nullable enable
namespace TheOne.Lifecycle.DI
{
    using VContainer;

    public static class LifecycleManagerVContainer
    {
        public static void RegisterLifecycleManager(this IContainerBuilder builder)
        {
            if (builder.Exists(typeof(ILifecycleManager), true)) return;
            builder.Register<VContainerLifecycleManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif