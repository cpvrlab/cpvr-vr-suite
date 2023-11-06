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

    protected override void Start()
    {
        base.Start();
        
        _sceneButtons.Add(default); // Dummy object to occupy the first index in the list so the button indexes match with the scene indexes
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var buttonObject = Instantiate(buttonPrefab, scrollviewContent);
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text =
                System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            var button = buttonObject.GetComponent<Button>();
            var index = i;
            button.onClick.AddListener(()=> { ChangeScene(index); });
            _sceneButtons.Add(buttonObject.GetComponent<Button>());
            _handMenuController.AddButtonSoundFeedback(button);
        }

        if (SceneManager.sceneCountInBuildSettings <= 1) return;
        _sceneButtons[1].interactable = false;
    }

    private void ChangeScene(int index)
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (index == currentIndex) return;
        _sceneButtons[currentIndex].interactable = true;
        _sceneButtons[index].interactable = false;
        _handMenuController.UnregisterDynamicPanels();
        SceneManager.LoadSceneAsync(index);
    }
}
