using UnityEngine;

namespace cpvr_vr_suite.Scripts.Util
{
    public class PersistentSingleton : MonoBehaviour
    {
        private static PersistentSingleton Instance { get; set; }

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);    
            }
        }

        private void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
}
