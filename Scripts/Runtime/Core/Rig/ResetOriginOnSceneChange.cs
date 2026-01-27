using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class ResetOriginOnSceneChange : MonoBehaviour
    {
        public Vector3 ResetPosition { get; set; }

        private void OnEnable()
        {
            ResetPosition = Vector3.zero;
            SceneManager.activeSceneChanged += (_, _) =>
            {
                transform.position = ResetPosition;
                ResetPosition = Vector3.zero;
            };
        }
    }
}
