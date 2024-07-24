using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionPanel : MonoBehaviour
{
    [SerializeField] GameObject m_sceneHandlerContainer;
    [SerializeField] bool m_fadeOnSceneChange;
    [SerializeField] GameObject m_buttonPrefab;
    [SerializeField] Transform m_scrollviewContent;
    readonly List<Button> m_sceneButtons = new();
    ISceneHandler m_sceneHandler;
    HandMenuController m_handMenuController;

    public IEnumerable<Button> SceneButtons { get => m_sceneButtons.AsReadOnly(); }

    void Awake()
    {
        m_sceneHandler = m_sceneHandlerContainer.GetComponent<ISceneHandler>();
    }

    void Start()
    {
        if (TryGetComponent<HandmenuPanel>(out var panel))
            m_handMenuController = panel.HandMenuController;
        InitializeScenes();
    }

    void OnEnable()
    {
        m_sceneHandler.SceneChangeStarted += FadeOut;
        m_sceneHandler.SceneChangeCompleted += FadeIn;
    }

    void OnDisable()
    {
        m_sceneHandler.SceneChangeStarted -= FadeOut;
        m_sceneHandler.SceneChangeCompleted -= FadeIn;
    }

    public void InitializeScenes()
    {
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var label = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
            var index = i;
            CreateSceneButton(label, m_sceneHandler.ChangeScene, index);
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
