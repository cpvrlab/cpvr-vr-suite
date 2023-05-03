using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollViewContent;

    [Header("UI Elements")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject debugPanel;
    
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var button = Instantiate(buttonPrefab, scrollViewContent);
            button.GetComponentInChildren<TextMeshProUGUI>().text =
                System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            var index = i;
            button.GetComponent<Button>().onClick.AddListener(()=> { ChangeScene(index); });
        }
    }

    public void OnCloseClick() => StartCoroutine(CloseMenu());

    public void OnDebugClick() => OpenPanel(debugPanel);

    public void OnBackClicked() => OpenPanel(mainPanel);

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
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(index);
    }
    
    private void OpenPanel(GameObject panel)
    {
        CloseAllPanels();
        panel.SetActive(true);
    }

    private void CloseAllPanels()
    {
        mainPanel.SetActive(false);
        debugPanel.SetActive(false);
    }

    private IEnumerator CloseMenu()
    {
        OpenPanel(mainPanel);
        _animator.Play("MenuClose");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }
}
