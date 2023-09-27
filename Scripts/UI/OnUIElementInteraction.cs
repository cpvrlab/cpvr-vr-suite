using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnUIElementInteraction : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private UnityEvent OnHover;
    [SerializeField] private UnityEvent OnDown;
    [SerializeField] private UnityEvent OnClick;

    public void OnPointerEnter(PointerEventData _) => OnHover?.Invoke();

    public void OnPointerDown(PointerEventData _) => OnDown?.Invoke();

    public void OnPointerClick(PointerEventData _) => OnClick.Invoke();

}
