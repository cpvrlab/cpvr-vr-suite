using UnityEngine;

namespace cpvrlab_vr_suite.Scripts.VR
{
    public class InputDevice : MonoBehaviour
    {
        public bool controllerInput;
        [SerializeField] private GameObject leftController;
        [SerializeField] private GameObject rightController;
        [SerializeField] private GameObject leftHand;
        [SerializeField] private GameObject rightHand;
        [SerializeField] private GameObject handVisualizer;

        private void Awake()
        {
            leftHand.SetActive(!controllerInput);
            rightHand.SetActive(!controllerInput);
            handVisualizer.SetActive(!controllerInput);
            leftController.SetActive(controllerInput);
            rightController.SetActive(controllerInput);
        }
    }
}
