using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.VR
{
    public class ResetOriginOnSceneChange : MonoBehaviour
    {
        private void OnEnable() =>
            SceneManager.activeSceneChanged += (_, _) => transform.position = Vector3.zero;
    }
}
