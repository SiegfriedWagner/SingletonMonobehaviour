using SiegfriedWagner.Singletons.Internal;
using UnityEngine;

namespace SiegfriedWagner.Singletons
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T: SingletonMonoBehaviour<T>
    {
        /// <summary>
        /// Called in Awake when object is not destroyed immediately.
        /// </summary>
        protected abstract void InheritorAwake();
        /// <summary>
        /// Called in OnDestroy when objects InheritorAwake was called before.
        /// </summary>
        protected abstract void InheritorOnDestroy();
        /// <summary>
        /// Called in OnValidate.
        /// </summary>
        protected abstract void InheritorOnValidate();
        private static T _instance = null;
        /// <summary>
        /// Returns existing singleton instance or creates new if there isn't any existing yet
        /// </summary>
        public static T Instance
        {
            get
            {
                if (!Instantiated && !ApplicationInfo.IsQuiting)
                {
                    _instance = UnitySingletonHelper.CreateOrFindInstance<T>();
                    Instantiated = true;
                }
                return _instance;
            }
        }
        /// <summary>
        /// Returns true if singleton instance is present in scene
        /// </summary>
        public static bool Instantiated { get; private set; }
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            var siblings = FindObjectsOfType<T>();
            if (siblings.Length > 1)
                Debug.LogError($"More than one instance of {typeof(SingletonMonoBehaviour<T>)} present in scene");
            InheritorOnValidate();
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
                _instance = (T) this;
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
    }
}
