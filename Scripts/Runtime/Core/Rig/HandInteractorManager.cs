using UnityEngine;

public class HandInteractorManager : MonoBehaviour
{
    [SerializeField] InteractorControl[] m_interactorControls;

    void Update()
    {
        foreach (var control in m_interactorControls)
            control.interactor.SetActive(control.IsActive);
    }

    public void BlockTeleport(bool value)
    {
        foreach (var item in m_interactorControls)
        {
            if (item.interactor.name == "Teleport Interactor")
                item.blocked = value;
        }
    }
}
