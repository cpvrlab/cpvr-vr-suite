using cpvr_vr_suite.Scripts.Runtime.Core;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    public class SetRigTransform : MonoBehaviour
    {
        void Start()
        {
            if (RigManager.Instance != null && RigManager.Instance.RigOrchestrator != null)
                RigManager.Instance.RigOrchestrator.Origin.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
}
