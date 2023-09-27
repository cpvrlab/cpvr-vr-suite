using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionPanel : MenuPanel
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollViewContent;
    private readonly List<Button> _sceneButtons = new();

    private void Awake()
    {
        _sceneButtons.Add(default); // Dummy object to occupy the first index in the list so the button indexes match with the scene indexes
        for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var button = Instantiate(buttonPrefab, scrollViewContent);
            button.GetComponentInChildren<TextMeshProUGUI>().text =
                System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            var index = i;
            button.GetComponent<Button>().onClick.AddListener(()=> { ChangeScene(index); });
            _sceneButtons.Add(button.GetComponent<Button>());
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
        SceneManager.LoadSceneAsync(index);
    }
}
