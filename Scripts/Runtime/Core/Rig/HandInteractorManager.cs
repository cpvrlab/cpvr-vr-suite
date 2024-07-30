using UnityEngine;

public class HandInteractorManager : MonoBehaviour
{
    [SerializeField] InteractorControl[] m_interactorControls;

    void Update()
    {
        foreach (var control in m_interactorControls)
            control.interactor.SetActive(control.IsActive);
    }
}
