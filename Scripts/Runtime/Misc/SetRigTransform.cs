using cpvr_vr_suite.Scripts.Runtime.Core;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    public class SetRigTransform : MonoBehaviour
    {
        void Start()
        {
            if (RigManager.Instance != null && RigManager.Instance.TryGet<XROrigin>(out var origin))
                origin.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
}
