using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SiegfriedWagner.Singletons.Tests.Runtime
{
    public class SceneScopedTests
    {
        private const string SceneScoped1Path = "Tests/Runtime/SceneScoped1";
        private const string SceneScoped2Path = "Tests/Runtime/SceneScoped2";

        [UnityTest]
        public IEnumerator TestSingletonInScene1()
        {
            SceneManager.LoadScene(SceneScoped1Path, LoadSceneMode.Single);
            yield return null;
            var scene = SceneManager.GetSceneByName(SceneScoped1Path);
            Scene activeScene = SceneManager.GetActiveScene();
            Assert.True(activeScene == scene, $"{SceneScoped1Path} is not an active scene, active scene is {activeScene.path}");
            var activeSceneSingleton = SceneScopedTestSingleton.GetActiveSceneInstance();
            var explicitSceneSingleton = SceneScopedTestSingleton.GetInstance(scene);
            Assert.AreEqual(activeSceneSingleton, explicitSceneSingleton);
        }

        [UnityTest]
        public IEnumerator TestLoadAdditive()
        {                                                                 
            SceneManager.LoadScene(SceneScoped1Path, LoadSceneMode.Single);
            yield return null;
            SceneManager.LoadScene(SceneScoped2Path, LoadSceneMode.Additive);
            yield return null;
            var scene1 = SceneManager.GetSceneByName(SceneScoped1Path);
            var scene2 = SceneManager.GetSceneByName(SceneScoped2Path);
            Assert.IsTrue(SceneScopedTestSingleton.IsInstantiated(scene1));
            Assert.IsTrue(SceneScopedTestSingleton.IsInstantiated(scene2));
        }

        [UnityTest]
        public IEnumerator TestObjectDestroy()
        {
            SceneManager.LoadScene(SceneScoped1Path, LoadSceneMode.Single);
            yield return null;
            var active = SceneScopedTestSingleton.GetActiveSceneInstance();
            Object.Destroy(active);
            yield return null;
            Assert.IsEmpty(SceneScopedTestSingleton.InstantiatedSingletonsScenes);
        }
    }
}
