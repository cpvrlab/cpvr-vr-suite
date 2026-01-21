using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionPanel : MonoBehaviour
{
    public IEnumerable<Button> SceneButtons { get => m_sceneButtons.AsReadOnly(); }

    [SerializeField] GameObject m_sceneHandlerContainer;
    [SerializeField] bool m_fadeOnSceneChange;
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Transform m_scrollviewContent;
    readonly List<Button> m_sceneButtons = new();
    ISceneHandler m_sceneHandler;
    HandMenuController m_handMenuController;

    LoadingIndicator m_loadingIndicator;

    void Awake()
    {
        m_sceneHandler = m_sceneHandlerContainer.GetComponent<ISceneHandler>();
    }

    void Start()
    {
        m_loadingIndicator = RigManager.Instance != null ? RigManager.Instance.Get<LoadingIndicator>() : null;

        m_sceneHandler.SceneChangeStarted += FadeOut;
        m_sceneHandler.SceneChangeCompleted += FadeIn;
        m_sceneHandler.SceneChangeStarted += m_loadingIndicator.StartLoading;
        m_sceneHandler.SceneChangeCompleted += m_loadingIndicator.StopLoading;

        if (TryGetComponent<HandmenuPanel>(out var panel))
            m_handMenuController = panel.HandMenuController;
        InitializeScenes();
    }

    void OnDestroy()
    {
        if (m_sceneHandler == null) return;

        m_sceneHandler.SceneChangeStarted -= FadeOut;
        m_sceneHandler.SceneChangeCompleted -= FadeIn;
        m_sceneHandler.SceneChangeStarted -= m_loadingIndicator.StartLoading;
        m_sceneHandler.SceneChangeCompleted -= m_loadingIndicator.StopLoading;
    }

    public void InitializeScenes()
    {
        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenename = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
            if (scenename.Contains("Bootstrap", StringComparison.OrdinalIgnoreCase))
                continue;
            var index = i;
            CreateSceneButton(scenename, m_sceneHandler.ChangeScene, index);
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
        if (m_handMenuController != null)
            m_handMenuController.AddButtonSoundFeedback(button);
        return button;
    }

    public void SetSceneHandler(ISceneHandler handler)
    {
        m_sceneHandler = handler;
        m_sceneHandler.SceneChangeStarted += FadeOut;
        m_sceneHandler.SceneChangeCompleted += FadeIn;
    }

    async void FadeOut()
    {
        if (!m_fadeOnSceneChange) return;
        await RigManager.Instance.Fade(Color.black, 0.75f);
    }

    async void FadeIn()
    {
        if (!m_fadeOnSceneChange) return;
        await RigManager.Instance.Fade(Color.clear, 0.75f);
    }
}
