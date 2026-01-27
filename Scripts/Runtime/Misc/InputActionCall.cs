using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    public class InputActionCall : MonoBehaviour
    {
        [SerializeField] InputActionProperty inputAction;

        public UnityEvent callEvent;

        void OnEnable() => inputAction.action.performed += _ => callEvent.Invoke();
        void OnDisable() => inputAction.action.performed -= _ => callEvent.Invoke();
    }
}
