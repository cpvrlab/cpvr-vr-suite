using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnUIElementHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UnityEvent OnHoverEnter;
    public void OnPointerEnter(PointerEventData eventData) {
        OnHoverEnter?.Invoke();
    }
}
