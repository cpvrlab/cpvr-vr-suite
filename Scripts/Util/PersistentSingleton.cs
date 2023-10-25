using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void Start(){
            if (SceneManager.sceneCountInBuildSettings <= 1) return;
            SceneManager.LoadSceneAsync(1);
        }

        private void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
}
