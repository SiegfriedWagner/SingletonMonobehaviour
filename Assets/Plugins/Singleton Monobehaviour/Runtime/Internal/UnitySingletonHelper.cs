using System;
using System.IO;
using SiegfriedWagner.Singletons.Attributes;
using SiegfriedWagner.Singletons.Exceptions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SiegfriedWagner.Singletons.Internal
{
	internal static class UnitySingletonHelper
	{
		public delegate T ObjectFactory<out T>();

		private const string GlobalSingletonContainerName = "GlobalSingletons";
		private static GameObject _globalSingletonContainer;
		private static GameObject _sceneSingletonContainer;

		/// <summary>
		///     Singleton container that is not destroyed on scene change
		/// </summary>
		public static GameObject GlobalSingletonsContainer
		{
			get
			{
				if (_globalSingletonContainer == null)
				{
					_globalSingletonContainer = new GameObject(GlobalSingletonContainerName);
					Object.DontDestroyOnLoad(_globalSingletonContainer);
				}

				return _globalSingletonContainer;
			}
		}

		public static ObjectFactory<T> ResolveFactoryMethodFor<T>() where T : MonoBehaviour
		{
			ObjectFactory<T> returnedValue = FindInstance<T>;
			var attributes = Attribute.GetCustomAttributes(typeof(T));
			var instantiatePrefabFound = false;
			foreach (var customAttribute in attributes)
				if (customAttribute is InstantiatedFromPrefabAttribute instantiatedFromPrefabAttribute)
				{
					instantiatePrefabFound = true;
					returnedValue = () =>
						FindInstanceOrCreateFromPrefab<T>(instantiatedFromPrefabAttribute.PathRelativeToResources);
				}
				else if (customAttribute is LazyInstantiatedAttribute && !instantiatePrefabFound)
				{
					returnedValue = FindInstanceOrCreate<T>;
				}

			return returnedValue;
		}

		public static void ValidateSingleton<T>() where T : MonoBehaviour
		{
			if (Application.isPlaying)
				return;
			var siblings = Object.FindObjectsOfType<T>();
			if (siblings.Length > 1)
				Debug.LogError($"More than one instance of {typeof(T)} opened in scene: {SceneManager.GetActiveScene().path}");
			if (Attribute.GetCustomAttribute(typeof(T), typeof(InstantiatedFromPrefabAttribute)) is
			    InstantiatedFromPrefabAttribute prefabAttribute)
			{

			}
		}

		public static T FindInstance<T>() where T : MonoBehaviour
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				throw new InvalidOperationException(
					$"{nameof(FindInstanceOrCreate)} called while application is not playing");
#endif
			var instance = Object.FindObjectOfType<T>();
			if (instance != null) return instance;

			throw new UnableToResolveException(
				$"Unable to find object of type {typeof(T).FullName} using {nameof(FindInstance)} method.");
		}

		public static T FindInstanceOrCreate<T>() where T : MonoBehaviour
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				throw new InvalidOperationException(
					$"{nameof(FindInstanceOrCreate)} called while application is not playing");
#endif
			var instance = Object.FindObjectOfType<T>();
			if (instance != null) return instance;

			var go = new GameObject(typeof(T).ToString());
			instance = go.AddComponent<T>();
			return instance;
		}

		public static T FindInstanceOrCreateFromPrefab<T>(string pathInResources) where T : MonoBehaviour
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				throw new InvalidOperationException(
					$"{nameof(FindInstanceOrCreate)} called while application is not playing");
#endif
			var instance = Object.FindObjectOfType<T>();
			if (instance != null) return instance;

			var prefab = Resources.Load<T>(pathInResources);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			if (prefab == null)
			{
				Debug.LogError(
					$"Missing prefab in resources: {pathInResources} or component in prefab, valid resource path is e.g." +
					Path.Combine(Directory.GetCurrentDirectory(), "Assets",
						"Resources", $"{pathInResources}.prefab"));
				throw new UnableToResolveException(
					$"Unable to find object of type {typeof(T).FullName} or create new using {nameof(FindInstanceOrCreateFromPrefab)} method by checking \"{pathInResources}\" resource path.");
			}
#endif
			instance = Object.Instantiate(prefab);
			return instance;
		}
	}
}