using System;
using System.Collections.Generic;
using System.IO;
using SiegfriedWagner.Singletons.Attributes;
using SiegfriedWagner.Singletons.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiegfriedWagner.Singletons
{
	public abstract class ScenePrefabSingletonMonoBehaviour<T> : MonoBehaviour where T : ScenePrefabSingletonMonoBehaviour<T>
	{
		static ScenePrefabSingletonMonoBehaviour()
		{
			if (Attribute.GetCustomAttribute(typeof(ScenePrefabSingletonMonoBehaviour<T>), typeof(PrefabPathAttribute)) is
			    PrefabPathAttribute prefabPathAttribute)
			{
				PrefabPath = prefabPathAttribute.PathRelativeToResources;
			}
			else
			{
				PrefabPath = Path.Combine(SingletonPrefabAssetPath, typeof(T).Name);
			}
		}

		private static readonly string PrefabPath;
		private const string SingletonPrefabAssetPath = "Singletons/";
		private static Dictionary<Scene, T> _sceneScopedSingletons = new();
		private bool _removeFromDict;

		public static IEnumerable<Scene> InstantiatedSingletonsScenes => _sceneScopedSingletons.Keys;
		
		/// <summary>
		/// Returns singleton instance related to currently active scene.
		/// </summary>
		/// <returns>Singleton instance. Value is never null unless application is closing.</returns>
		public static T GetActiveSceneInstance()
		{
			return GetInstance(SceneManager.GetActiveScene());
		}

		/// <summary>
		/// Returns singleton instance related to specific scene. 
		/// </summary>
		/// <param name="scene"></param>
		/// <returns>Singleton </returns>
		/// <exception cref="InvalidOperationException">Thrown when argument scene is not loaded.</exception>
		public static T GetInstance(Scene scene)
		{
			if (!_sceneScopedSingletons.TryGetValue(scene, out var instance))
			{
				if (!scene.isLoaded)
					throw new InvalidOperationException("Selected scene is not loaded");
				var activeScene = SceneManager.GetActiveScene();
				var switchScene = false;
				if (scene != activeScene)
				{
					SceneManager.SetActiveScene(scene);
					switchScene = true;
				}

				(instance, _) = UnitySingletonHelper.CreateOrFindInstance<T>(PrefabPath);
				if (switchScene)
					SceneManager.SetActiveScene(activeScene);
				_sceneScopedSingletons.Add(scene, instance);
			}

			return instance;
		}

		public static bool IsInstantiated(Scene scene)
		{
			return _sceneScopedSingletons.ContainsKey(scene);
		}

		protected virtual void InheritorAwake(bool isImmediatelyDestroyed)
		{
		}

		protected virtual void InheritorOnValidate()
		{
		}

		protected virtual void InheritorOnDestroy(bool isImmediatelyDestroyed)
		{}
		private void OnValidate()
		{
			if (Application.isPlaying)
				return;
			var pathToCheck = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Resources",
				$"{PrefabPath}.prefab");
			if (!File.Exists(pathToCheck))
				Debug.LogError($"Missing prefab in {pathToCheck}, may cause errors in runtime");
			var siblings = FindObjectsOfType<T>();
			if (siblings.Length > 1)
				Debug.LogError($"More than one instance of {typeof(T)} present in scene");
			InheritorOnValidate();
		}

		private void Awake()
		{
			if (_sceneScopedSingletons.TryGetValue(gameObject.scene, out var dictInst) && dictInst != this)
			{
				Destroy(this);
				Debug.LogWarning($"Instance of {typeof(ScenePrefabSingletonMonoBehaviour<T>)} already present in the scene");
				InheritorAwake(true);
			}
			else
			{
				_removeFromDict = true;
				_sceneScopedSingletons.Add(gameObject.scene, (T)this);
				InheritorAwake(false);
			}
		}

		private void OnDestroy()
		{
			if (_removeFromDict)
			{
				InheritorAwake(false);
				_sceneScopedSingletons.Remove(gameObject.scene);
			}
			else
				InheritorAwake(true);
		}
	}
}