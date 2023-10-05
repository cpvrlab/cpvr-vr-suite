using UnityEngine;
using UnityEngine.Events;

public class GazeEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _hoverStartEvent;
    [SerializeField] private UnityEvent _hoverEndEvent;
    [SerializeField] private UnityEvent _selectStartEvent;
    [SerializeField] private UnityEvent _selectEndEvent;

    public void OnHoverEnter() => _hoverStartEvent?.Invoke();
    public void OnHoverExit() => _hoverEndEvent?.Invoke();
    public void OnSelectEnter() => _selectStartEvent?.Invoke();
    public void OnSelectExit() => _selectEndEvent?.Invoke();
}
