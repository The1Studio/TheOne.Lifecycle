#if THEONE_DI
#nullable enable
namespace TheOne.Lifecycle.DI
{
    using TheOne.DI;

    public static class LifecycleManagerDI
    {
        public static void AddLifecycleManager(this DependencyContainer container)
        {
            if (container.Contains<ILifecycleManager>()) return;
            container.AddInterfaces<DILifecycleManager>();
        }
    }
}
#endif