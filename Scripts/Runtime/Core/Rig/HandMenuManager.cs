using UnityEngine;

public class HandMenuManager : InteractorManager
{
    [field: SerializeField] public GameObject HandMenu { get; private set; }
    [field: SerializeField] public HandMenuController HandMenuController { get; private set; }
   
    bool m_operative = true;
    public bool Operative 
    { 
        get => m_operative; 
        set
        {
            m_operative = value;
            if (HandMenu != null)
                HandMenu.SetActive(m_operative && !m_blocked);
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
        }
    }

    void Awake() => Initialize();
    
    void Initialize()
    {
        Blocked = false;
        Operative = true;
        HandMenu.SetActive(Operative && !Blocked);
    }
}
