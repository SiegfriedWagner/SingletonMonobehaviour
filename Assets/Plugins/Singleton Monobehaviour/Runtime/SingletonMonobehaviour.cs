using SiegfriedWagner.Singletons.Internal;
using UnityEngine;

namespace SiegfriedWagner.Singletons
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		private static readonly UnitySingletonHelper.ObjectFactory<T> FactoryMethod;

		static SingletonMonoBehaviour()
		{
			FactoryMethod = UnitySingletonHelper.ResolveFactoryMethodFor<T>();
		}

		private static T _instance;

		/// <summary>
		///     Returns true if singleton instance is present in scene
		/// </summary>
		// ReSharper disable once StaticMemberInGenericType - it's fine, one static field per singleton an intended behaviour
		public static bool Instantiated { get; private set; }


		/// <summary>
		/// Returns existing singleton instance or creates new if there isn't any existing yet
		/// </summary>
		/// <exception cref="Exceptions.UnableToResolveException">Thrown when factory method is unable to find or create singleton instance.</exception>
		public static T GetInstance()
		{
			if (!Instantiated && !ApplicationInfo.IsQuiting)
			{
				_instance = FactoryMethod();
				Instantiated = true;
			}

			return _instance;
		}


		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Destroy(this);
				Debug.LogWarning($"Instance of {typeof(SingletonMonoBehaviour<T>)} already present in the runtime");
			}
			else
			{
				Instantiated = true;
				_instance = (T)this;
				_instance.transform.SetParent(UnitySingletonHelper.GlobalSingletonsContainer.transform);
				InheritorAwake();
			}
		}

		private void OnDestroy()
		{
			if (_instance == this)
			{
				Instantiated = false;
				_instance = null;
				InheritorOnDestroy();
			}
		}

		private void OnValidate()
		{
			if (Application.isPlaying)
				return;
			var siblings = FindObjectsOfType<T>();
			if (siblings.Length > 1)
				Debug.LogError($"More than one instance of {typeof(SingletonMonoBehaviour<T>)} present in scene");
			InheritorOnValidate();
		}

		/// <summary>
		///     Called in Awake when object is not destroyed immediately.
		/// </summary>
		protected virtual void InheritorAwake()
		{
		}

		/// <summary>
		///     Called in OnDestroy when objects InheritorAwake was called before.
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