using SiegfriedWagner.Singletons.Internal;
using UnityEngine;

namespace SiegfriedWagner.Singletons
{
	/// <summary>
	///     A singleton inheriting from MonoBehaviour.
	/// </summary>
	/// <typeparam name="T">Class type.</typeparam>
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		private static readonly UnitySingletonHelper.ObjectFactory<T> FactoryMethod;

		private static T _instance;

		static SingletonMonoBehaviour()
		{
			FactoryMethod = UnitySingletonHelper.ResolveFactoryMethodFor<T>();
		}

		/// <summary>
		///     Returns true if singleton instance is present in scene
		/// </summary>
		// ReSharper disable once StaticMemberInGenericType - it's fine, one static field per singleton an intended behaviour
		public static bool Instantiated { get; private set; }


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
			UnitySingletonHelper.ValidateSingleton<T>();
			InheritorOnValidate();
		}


		/// <summary>
		///     Returns existing singleton instance.
		///     If no singleton instances is register at the beginning of this method call then factory method is used
		///     to resolve proper instance.
		/// </summary>
		/// <exception cref="Exceptions.UnableToResolveException">
		///     Thrown when factory method is unable to find or create singleton instance.
		/// </exception>
		public static T GetInstance()
		{
			if (!Instantiated && !ApplicationInfo.IsQuiting)
			{
				_instance = FactoryMethod();
				Instantiated = true;
			}

			return _instance;
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