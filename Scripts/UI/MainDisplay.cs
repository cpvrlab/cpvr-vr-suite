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
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var button = Instantiate(buttonPrefab, scrollViewContent);
                button.GetComponentInChildren<TextMeshProUGUI>().text =
                    System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                var index = i;
                button.GetComponent<Button>().onClick.AddListener(()=> { ChangeScene(index); });
            }
        }

        public void OnCloseClick() => menuController.CloseMenu();

        public void OnSettingsClick() => menuController.OpenPanel(1);

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
            menuController.gameObject.SetActive(false);
            SceneManager.LoadSceneAsync(index);
        }
    }
}
