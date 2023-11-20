using UnityEngine;

public class SetRigTransform : MonoBehaviour
{
    void Start()
    {
        if (RigManager.Instance != null && RigManager.Instance.XrOrigin != null)
            RigManager.Instance.XrOrigin.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
