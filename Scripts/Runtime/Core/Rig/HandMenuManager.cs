using UnityEngine;
using UnityEngine.UI;

public class HandMenuManager : InteractorManager
{
    [field: SerializeField] public GameObject HandMenu { get; private set; }
    [field: SerializeField] public HandMenuController HandMenuController { get; private set; }
    [field: SerializeField] public GameObject HandMenuButton { get; private set; }
    [field: SerializeField] public Button HandButton { get; private set; }
    bool m_operative = true;
    public bool Operative 
    { 
        get => m_operative; 
        set
        {
            m_operative = value;
            if (HandMenu != null)
                HandMenu.SetActive(m_operative && !m_blocked);
            if (HandMenuButton != null)
                HandMenuButton.SetActive(m_operative && !m_blocked);
        }
    }
    bool m_blocked = false;
    public bool Blocked 
    { 
        get => m_blocked; 
        set
        {
            m_blocked = value;
            if (HandMenu != null)
                HandMenu.SetActive(m_operative && !m_blocked);
            if (HandMenuButton != null)
                HandMenuButton.SetActive(m_operative && !m_blocked);
        } 
    }

    void Awake() => Initialize();

    void Start()
    {
        if (HandButton != null)
            HandButton.onClick.AddListener(() => HandMenu.SetActive(!HandMenu.activeSelf));
    }
    
    void Initialize()
    {
        Blocked = false;
        Operative = true;
        HandMenu.SetActive(Operative && !Blocked);
        HandMenuButton.SetActive(Operative && !Blocked);
    }
}
