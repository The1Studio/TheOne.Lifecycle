#if THEONE_ZENJECT
#nullable enable
namespace TheOne.Lifecycle.DI
{
    using Zenject;

    public static class LifecycleManagerZenject
    {
        public static void BindLifecycleManager(this DiContainer container)
        {
            if (container.HasBinding<ILifecycleManager>()) return;
            container.BindInterfacesTo<ZenjectLifecycleManager>().AsSingle();
        }
    }
}
#endif