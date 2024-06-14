using Unity.Netcode;
using UnityEngine;

namespace Util {
    /// <summary>
    /// Same Singleton as Singleton<T> but using NetworkBehaviour instead of MonoBehaviour.
    /// </summary>
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour {
        public static T Instance { get; private set; }
        [SerializeField] private bool _isPersistent;

        protected virtual void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this as T;
                if (_isPersistent)
                    DontDestroyOnLoad(this);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
}