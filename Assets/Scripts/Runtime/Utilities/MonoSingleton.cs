using UnityEngine;

namespace FiveBee.Runtime.Utilities
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Instance => GetInstance();
        
        private static bool _isInitialized;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"Instance of {nameof(T)} already exists, destroying {gameObject.name}...");
                Destroy(gameObject);

                return;
            }
            
            if (!_isInitialized)
            {
                _isInitialized = true;
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            
        }

        private static T GetInstance()
        {
            if (_instance == null)
            {
                var insts = FindObjectsByType<T>(FindObjectsSortMode.None);
                
                if (insts.Length > 0)
                {
                    _instance = insts[0];
                }

                if (insts.Length > 1)
                {
                    for (var i = 1; i < insts.Length; i++)
                    {
                        Destroy(insts[i]);
                    }
                }
            }
                
            if (_instance == null)
            {
                Debug.LogWarning($"Instance of {nameof(T)} not found, creating new one...");
                _instance = new GameObject("Client").AddComponent<T>();
                
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    _instance.Initialize();
                }
            }
                
            return _instance;
        }
    }
}