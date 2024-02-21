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
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Transform m_scrollviewContent;
    readonly List<Button> m_sceneButtons = new();

    public IEnumerable<Button> SceneButtons {get => m_sceneButtons.AsReadOnly();}

    protected override void Start()
    {
        base.Start();
        
        m_sceneButtons.Add(default); // Dummy object to occupy the first index in the list so the button indexes match with the scene indexes
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var label = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
            var index = i;
            CreateSceneButton(label, ChangeScene, index);
        }
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

    async void ChangeScene(int index)
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
