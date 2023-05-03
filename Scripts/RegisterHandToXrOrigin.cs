using UnityEngine;

namespace cpvrlab_vr_suite.Scripts
{
    public class RegisterHandToXrOrigin : MonoBehaviour
    {
        [SerializeField] private bool isRightHand;
        private void Start()
        {
            var origin = GameObject.Find("XR Origin");
            if (origin == null)
            {
                Debug.LogError("XR Origin not found!");
                return;
            }

            var playerBehaviourScript = origin.GetComponent<PlayerBehaviour>();
            playerBehaviourScript.RegisterHand(transform, isRightHand);
        }
    }
}
