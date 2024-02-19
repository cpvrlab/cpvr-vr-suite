using UnityEngine;

public abstract class HandView : View
{
    [SerializeField] protected PanelType panelType = PanelType.Static;
    [SerializeField] Sprite m_sprite;
    public Sprite Sprite { get => m_sprite; }
}
