using Framework.Common;
using UnityEngine;

namespace Framework.Scripts.Common
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                return s_Instance;
            }
            private set
            {
                s_Instance = value;
            }
        }

        protected static bool IsApplicationQuitting { get; private set; }

        protected virtual void OnEnable()
        {
            if (s_Instance != null && s_Instance != this)
            {
                console.warn(this, $"Singleton {typeof(T).Name} exists, destroying this instance!", this.gameObject);
                Destroy(gameObject);
                return; 
            }
            Instance = this as T;
            OnAfterEnable(); 
        }

        protected virtual void OnDisable()
        {
            OnAfterDisable();
        }

        protected virtual void OnAfterEnable()
        {
            
        }
        protected virtual void OnAfterDisable()
        {

        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            IsApplicationQuitting = false;
        }
        
        private void OnApplicationQuit()
        {
            IsApplicationQuitting = true;
        }

    }
}