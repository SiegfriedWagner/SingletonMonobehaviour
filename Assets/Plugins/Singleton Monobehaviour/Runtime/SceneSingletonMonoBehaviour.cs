using System;
using System.Collections.Generic;
using SiegfriedWagner.Singletons.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiegfriedWagner.Singletons
{
	/// <summary>
	///     A scene scoped singleton inheriting from MonoBehaviour.
	///     Scene scoped singleton is a mono behavior that can have at most a single instance in each scene in project.
	/// </summary>
	/// <typeparam name="T">Class type.</typeparam>
	public class SceneSingletonMonoBehaviour<T> : MonoBehaviour where T : SceneSingletonMonoBehaviour<T>
	{
		private static readonly UnitySingletonHelper.ObjectFactory<T> FactoryMethod;
		private static readonly Dictionary<Scene, T> SceneScopedSingletons = new();
		private bool _removeFromDict;

		static SceneSingletonMonoBehaviour()
		{
			FactoryMethod = UnitySingletonHelper.ResolveFactoryMethodFor<T>();
		}

		/// <summary>
		///     Enumerable of all scenes that contain singleton instance.
		/// </summary>
		public static IEnumerable<Scene> InstantiatedSingletonsScenes => SceneScopedSingletons.Keys;

		/// <summary>
		///     Enumerable of all available singleton instances.
		/// </summary>
		public static IEnumerable<T> InstantiatedSingletons => SceneScopedSingletons.Values;

		private void Awake()
		{
			if (SceneScopedSingletons.TryGetValue(gameObject.scene, out var dictInst) && dictInst != this)
			{
				Destroy(this);
				Debug.LogWarning($"Instance of {typeof(SceneSingletonMonoBehaviour<T>)} already present in the scene");
			}
			else
			{
				_removeFromDict = true;
				SceneScopedSingletons.Add(gameObject.scene, (T)this);
				InheritorAwake();
			}
		}

		private void OnDestroy()
		{
			if (_removeFromDict)
				try
				{
					InheritorOnDestroy();
				}
				finally
				{
					SceneScopedSingletons.Remove(gameObject.scene);
				}
		}

		private void OnValidate()
		{
			UnitySingletonHelper.ValidateSingleton<T>();
			InheritorOnValidate();
		}

		/// <summary>
		///     Returns singleton instance related to currently active scene.
		/// </summary>
		/// <returns>Singleton instance. Value is never null unless application is closing.</returns>
		public static T GetActiveSceneInstance()
		{
			return GetInstance(SceneManager.GetActiveScene());
		}

		/// <summary>
		///     Returns singleton instance related to specific scene.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns>Singleton </returns>
		/// <exception cref="InvalidOperationException">Thrown when argument scene is not loaded.</exception>
		public static T GetInstance(Scene scene)
		{
			if (!SceneScopedSingletons.TryGetValue(scene, out var instance))
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

				instance = FactoryMethod();
				if (switchScene)
					SceneManager.SetActiveScene(activeScene);
				SceneScopedSingletons.TryAdd(scene, instance);
			}

			return instance;
		}

		/// <summary>
		///     Checks if loaded scene contains singleton instance.
		/// </summary>
		/// <param name="scene">Checked scene.</param>
		/// <returns>True if singleton is instantiated in given loaded scene, otherwise false.</returns>
		public static bool IsInstantiated(Scene scene)
		{
			if (!scene.isLoaded)
				return false;
			return SceneScopedSingletons.ContainsKey(scene);
		}

		/// <summary>
		///     Called in Awake when object is not destroyed immediately.
		/// </summary>
		protected virtual void InheritorAwake()
		{
		}

		/// <summary>
		///     Called in OnDestroy.
		///     Always called once in object lifetime and called only if complementary call to InheritorAwake was invoked.
		/// </summary>
		protected virtual void InheritorOnDestroy()
		{
		}

		/// <summary>
		///     Called in OnValidate.
		/// </summary>
		protected virtual void InheritorOnValidate()
		{
		}
	}
}