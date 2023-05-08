using UnityEngine;
using UnityEngine.InputSystem;

namespace cpvrlab_vr_suite.Scripts.VR
{
    public class AnimateController : MonoBehaviour
    {
        [SerializeField] private InputActionProperty pinchAction;
        [SerializeField] private InputActionProperty gripAction;
        private Animator _animator;
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");

        private void Awake() => _animator = GetComponent<Animator>();

        private void Update()
        {
            var triggerValue = pinchAction.action.ReadValue<float>();
            _animator.SetFloat(Trigger, triggerValue);

            var gripValue = gripAction.action.ReadValue<float>();
            _animator.SetFloat(Grip, gripValue);
        }
    }
}
