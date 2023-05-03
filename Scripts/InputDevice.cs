using UnityEngine;

public class InputDevice : MonoBehaviour
{
    public bool controllerInput;
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
    [SerializeField] private GameObject handVisualizer;

    private void Awake()
    {
        handVisualizer.SetActive(!controllerInput);
        leftController.SetActive(controllerInput);
        rightController.SetActive(controllerInput);
    }
}
