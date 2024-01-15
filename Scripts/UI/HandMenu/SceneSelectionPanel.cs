using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionPanel : MenuPanel
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollviewContent;
    private readonly List<Button> _sceneButtons = new();

    public IEnumerable<Button> SceneButtons {get => _sceneButtons.AsReadOnly();}

    protected override void Start()
    {
        base.Start();
        
        _sceneButtons.Add(default); // Dummy object to occupy the first index in the list so the button indexes match with the scene indexes
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var label = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
            var index = i;
            CreateSceneButton(label, ChangeScene, index);
        }
    }

    public Button CreateSceneButton<T>(string label, Action<T> callback, T argument)
    {
        var buttonObject = Instantiate(buttonPrefab, scrollviewContent);
        buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = label;
        var button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(() => callback.Invoke(argument));
        _sceneButtons.Add(button);
        _handMenuController.AddButtonSoundFeedback(button);
        return button;
    }

    private void ChangeScene(int index)
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (index == currentIndex) return;
        _handMenuController.UnregisterDynamicPanels();
        SceneManager.LoadSceneAsync(index);
    }
}
