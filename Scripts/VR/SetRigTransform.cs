using System.Collections;
using UnityEngine;

public class SetRigTransform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            xrOrigin.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
}
