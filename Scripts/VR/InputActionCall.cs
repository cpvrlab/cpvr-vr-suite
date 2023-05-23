using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace cpvrlab_vr_suite.Scripts.VR
{
    public class InputActionCall : MonoBehaviour
    {
        [SerializeField] private InputActionProperty inputAction;

        public UnityEvent callEvent;

        private void OnEnable() => inputAction.action.performed += _ => callEvent.Invoke();
    }
}
