using System;
using System.IO;
using SiegfriedWagner.Singletons.Attributes;
using SiegfriedWagner.Singletons.Internal;
using UnityEngine;

namespace SiegfriedWagner.Singletons
{
	/// <summary>
	/// Singleton with prefab stored in Resources which may be loaded on demand lazily.
	/// </summary>
	public abstract class PrefabSingletonMonoBehaviour<T> : MonoBehaviour where T : PrefabSingletonMonoBehaviour<T>
	{
		static PrefabSingletonMonoBehaviour()
		{
			if (Attribute.GetCustomAttribute(typeof(PrefabSingletonMonoBehaviour<T>), typeof(PrefabPathAttribute)) is
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

		private static T _instance;

		/// <summary>
		/// Returns true if singleton instance is present in scene
		/// </summary>
		public static bool Instantiated { get; private set; }

		/// <summary>
		/// Returns existing singleton instance or creates new if there isn't any existing yet
		/// </summary>
		public static T Instance
		{
			get
			{
				if (!Instantiated && !ApplicationInfo.IsQuiting)
				{
					(_instance, Instantiated) = UnitySingletonHelper.CreateOrFindInstance<T>(PrefabPath);
				}
				return _instance;
			}
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
			if (_instance != null && _instance != this)
			{
				Destroy(this);
				Debug.LogWarning($"Instance of {typeof(T)} already present in the scene");
				InheritorAwake(true);
			}
			else
			{
				Instantiated = true;
				_instance = (T) this;
				_instance.transform.SetParent(UnitySingletonHelper.GlobalSingletonsContainer.transform);
				InheritorAwake(false);
			}
		}

		protected virtual void OnDestroy()
		{
			if (Instantiated && _instance == this)
			{
				Instantiated = false;
				_instance = null;
				InheritorOnDestroy(false);
			}
			else
				InheritorOnDestroy(true);
		}
	}
}