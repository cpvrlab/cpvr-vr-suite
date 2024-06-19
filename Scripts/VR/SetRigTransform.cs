using UnityEngine;

public class SetRigTransform : MonoBehaviour
{
    void Start()
    {
        if (RigManager.Instance != null && RigManager.Instance.RigOrchestrator != null)
            RigManager.Instance.RigOrchestrator.Origin.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
