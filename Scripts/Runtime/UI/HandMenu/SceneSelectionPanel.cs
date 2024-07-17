using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionPanel : MonoBehaviour
{
    public delegate void OnEnableHandler();
    public OnEnableHandler onEnableHandler;
    [SerializeField] HandMenuController m_handmenuController;
    [SerializeField] bool m_fadeOnSceneChange;
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Transform m_scrollviewContent;
    readonly List<Button> m_sceneButtons = new();

    public IEnumerable<Button> SceneButtons {get => m_sceneButtons.AsReadOnly();}

    void Start() => InitializeScenes();

    public void InitializeScenes()
    {
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var label = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
            var index = i;
            CreateSceneButton(label, ChangeScene, index);
        }
    }

    public void ClearSceneButtons()
    {
        foreach (var button in m_sceneButtons)
            Destroy(button.gameObject);

        m_sceneButtons.Clear();
    }

    public Button CreateSceneButton<T>(string label, Action<T> callback, T argument)
    {
        var buttonObject = Instantiate(m_buttonPrefab, m_scrollviewContent);
        buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = label;
        var button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(() => callback.Invoke(argument));
        m_sceneButtons.Add(button);
        m_handmenuController.AddButtonSoundFeedback(button);
        return button;
    }

    public void RemoveDynamicPanels() => m_handmenuController.UnregisterDynamicPanels();

    public virtual async void ChangeScene(int index)
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (index == currentIndex) return;
        RemoveDynamicPanels();

        if (m_fadeOnSceneChange)
            await RigManager.Instance.Fade(Color.black, 0.75f);
        
        SceneManager.LoadSceneAsync(index).completed += async _ =>
        {
            if (m_fadeOnSceneChange)
                await RigManager.Instance.Fade(Color.clear, 0.75f);
        };
    }

    void OnEnable() => onEnableHandler?.Invoke();
}
