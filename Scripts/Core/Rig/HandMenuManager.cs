using UnityEngine;
using UnityEngine.UI;

public class HandMenuManager : InteractorManager
{
    [SerializeField] GameObject m_handMenu;
    [SerializeField] GameObject m_handMenuButton;
    [SerializeField] Button m_handButton;
    bool m_operative = true;
    public bool Operative 
    { 
        get => m_operative; 
        set
        {
            m_operative = value;
            if (m_handMenu != null)
                m_handMenu.SetActive(m_operative && !m_blocked);
            if (m_handMenuButton != null)
                m_handMenuButton.SetActive(m_operative && !m_blocked);
        }
    }
    bool m_blocked = false;
    public bool Blocked 
    { 
        get => m_blocked; 
        set
        {
            m_blocked = value;
            if (m_handMenu != null)
                m_handMenu.SetActive(m_operative && !m_blocked);
            if (m_handMenuButton != null)
                m_handMenuButton.SetActive(m_operative && !m_blocked);
        } 
    }

    void Awake() => Initialize();

    void Start()
    {
        if (m_handButton != null)
            m_handButton.onClick.AddListener(() => m_handMenu.SetActive(!m_handMenu.activeSelf));
    }
    
    void Initialize()
    {
        Blocked = false;
        Operative = true;
        m_handMenu.SetActive(Operative && !Blocked);
        m_handMenuButton.SetActive(Operative && !Blocked);
    }
}
