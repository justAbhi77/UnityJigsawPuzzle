using UnityEngine;

namespace Patterns
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = FindAnyObjectByType<T>();
                if (_instance) return _instance;

                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
