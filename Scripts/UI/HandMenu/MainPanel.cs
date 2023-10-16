using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MenuPanel
{
    [SerializeField] private Button _dynamicButton;

    public void EnableDynamicButton(MenuPanel panel, int index)
    {
        if (_dynamicButton.transform.GetChild(0).TryGetComponent<Image>(out var image))
        {
            image.sprite = panel.Sprite;
        }

        _dynamicButton.onClick.RemoveAllListeners();
        _dynamicButton.onClick.AddListener(() => OnChangePanel(index));
        _dynamicButton.gameObject.SetActive(true);
        _handMenuController.AddButtonSoundFeedback(_dynamicButton);
    }
    public void OnChangePanel(int index) => _handMenuController.OpenPanel(index);
}
