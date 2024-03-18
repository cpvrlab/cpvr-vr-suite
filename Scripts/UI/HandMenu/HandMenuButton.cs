using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandMenuButton : MonoBehaviour
{
    [SerializeField] Image m_spriteImage;
    public Sprite Sprite
    {
        get => m_spriteImage != null ? m_spriteImage.sprite : null;
        set
        {
            if (m_spriteImage == null) return;

            m_spriteImage.sprite = value;
            m_spriteImage.gameObject.SetActive(value != null);
        }
    }
    [SerializeField] TMP_Text m_text;
    public string Text
    {
        get => m_text != null ? m_text.text : string.Empty;
        set
        {
            if (m_text != null)
                m_text.text = value;
        }
    }
    [SerializeField] Button m_button;
    public Button Button 
    { 
        get => m_button; 
        private set => m_button = value;
    }
}
