using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    public class ToggleGameObject : MonoBehaviour
    {
        public void ToggleObject(GameObject go) => go.SetActive(!go.activeSelf);
    }
}
