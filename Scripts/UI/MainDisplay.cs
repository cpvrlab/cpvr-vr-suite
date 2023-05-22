using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace cpvrlab_vr_suite.Scripts.UI
{
    public class MainDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private MenuController menuController;

        private void Awake()
        {
            if (SceneManager.sceneCountInBuildSettings > 1)
                SceneManager.LoadSceneAsync(1);
            for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var button = Instantiate(buttonPrefab, scrollViewContent);
                button.GetComponentInChildren<TextMeshProUGUI>().text =
                    System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                var index = i;
                button.GetComponent<Button>().onClick.AddListener(()=> { ChangeScene(index); });
            }
        }

        public void OnQuitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        
        private void ChangeScene(int index)
        {
            if (index == SceneManager.GetActiveScene().buildIndex) return;
            SceneManager.LoadSceneAsync(index);
        }
    }
}
