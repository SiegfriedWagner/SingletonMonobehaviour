using SiegfriedWagner.Singletons.Attributes;

namespace SiegfriedWagner.Singletons.Tests.Runtime
{
    [LazyInstantiated]
    public class LazySingleton : SingletonMonoBehaviour<LazySingleton> // don't instantiate this
    {
    }
}