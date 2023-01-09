using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SiegfriedWagner.Singletons.Internal
{
	internal static class UnitySingletonHelper
	{
		private const string globalSingletionContainerName = "GlobalSingletons";
		private static GameObject globalSingletonContainer;
		private static GameObject sceneSingletonContainer;
		/// <summary>
		/// Singleton container that is not destroyed on scene change
		/// </summary>
		internal static GameObject GlobalSingletonsContainer
		{
			get
			{
				if (globalSingletonContainer == null)
				{
					globalSingletonContainer = new GameObject(globalSingletionContainerName);
					Object.DontDestroyOnLoad(globalSingletonContainer);
				}
				return globalSingletonContainer;
			}
		}
		
		public static T CreateOrFindInstance<T>() where T : Component
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				throw new InvalidOperationException(
					$"{nameof(CreateOrFindInstance)} called while application is not playing");
#endif
			var instance = Object.FindObjectOfType<T>();
			if (instance != null)
			{
				return instance;
			}

			var go = new GameObject(typeof(T).ToString());
			instance = go.AddComponent<T>();
			return instance;
		}

		public static (T instance, bool instantiated) CreateOrFindInstance<T>(string pathInResources) where T : Component
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				throw new InvalidOperationException(
					$"{nameof(CreateOrFindInstance)} called while application is not playing");
#endif
			var instance = Object.FindObjectOfType<T>();
			if (instance != null)
			{
				return (instance, true);
			}

			var prefab = Resources.Load<T>(pathInResources);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			if (prefab == null)
			{
				Debug.LogError(
					$"Missing prefab in resources: {pathInResources} or component in prefab, valid resource path is e.g." +
					Path.Combine(Directory.GetCurrentDirectory(), "Assets",
						"Resources", $"{pathInResources}.prefab"));
				return (null, false);
			}
#endif
			instance = Object.Instantiate(prefab);
			return (instance, instance != null);
		}
	}
}