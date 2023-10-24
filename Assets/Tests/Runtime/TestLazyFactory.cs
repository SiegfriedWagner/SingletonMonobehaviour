using NUnit.Framework;
using UnityEngine;

namespace SiegfriedWagner.Singletons.Tests.Runtime
{
    public class TestLazyFactory
    {
        [Test]
        public void TestMultipleCallsToSameUninstancedLazySingleton()
        {
            var lazyInstance = LazySingleton.GetInstance();
            var lazySingleton = LazySingleton.GetInstance();
            var itemsInScene = Object.FindObjectsOfType<LazySingleton>(true);
            Assert.AreEqual(1, itemsInScene.Length);
        }
        
        [Test]
        public void TestMultipleCallsToSameUninstancedLazySceneSingleton()
        {
            var lazyInstance = LazySceneScopedSingleton.GetActiveSceneInstance();
            var lazySingleton = LazySceneScopedSingleton.GetActiveSceneInstance();
            var itemsInScene = Object.FindObjectsOfType<LazySceneScopedSingleton>(true);
            Assert.AreEqual(1, itemsInScene.Length);
        }
    }
}