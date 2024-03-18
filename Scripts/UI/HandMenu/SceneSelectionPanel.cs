using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionPanel : MenuPanel
{
    public delegate void OnEnableHandler();
    public OnEnableHandler onEnableHandler;
    [SerializeField] bool m_fadeOnSceneChange;
    [SerializeField] int m_startSceneIndex = 1;
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Transform m_scrollviewContent;
    readonly List<Button> m_sceneButtons = new();

    public IEnumerable<Button> SceneButtons {get => m_sceneButtons.AsReadOnly();}

    protected override void Start()
    {
        base.Start();
        
        for (var i = m_startSceneIndex; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var label = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
            var index = i;
            CreateSceneButton(label, ChangeScene, index);
        }

        SceneManager.activeSceneChanged += (_, scene) =>
        {
            if (handMenuController.TryGetMenuPanel<MainPanel>(out var panel))
                panel.Title = scene.name;
        };
    }

    public Button CreateSceneButton<T>(string label, Action<T> callback, T argument)
    {
        var buttonObject = Instantiate(m_buttonPrefab, m_scrollviewContent);
        buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = label;
        var button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(() => callback.Invoke(argument));
        m_sceneButtons.Add(button);
        handMenuController.AddButtonSoundFeedback(button);
        return button;
    }

    public void RemoveDynamicPanels() => handMenuController.UnregisterDynamicPanels();

    protected virtual async void ChangeScene(int index)
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (index == currentIndex) return;
        RemoveDynamicPanels();

        if (m_fadeOnSceneChange)
            await RigManager.Instance.Fade(Color.black, 2f);
        
        SceneManager.LoadSceneAsync(index);
        
        if (m_fadeOnSceneChange)
            await RigManager.Instance.Fade(Color.clear, 2f);
    }

    void OnEnable() => onEnableHandler?.Invoke();
}
