using System;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.VR
{
    public class RayCastOriginBehaviour : MonoBehaviour
    {
        [SerializeField] private bool isRight;
        [SerializeField] private float additionalPitchAngleDeg = -15.0f;
        private Transform _headTransform;
        private Vector3 _directionOffset;

        private void Start()
        {
            _headTransform = GameObject.Find("XR Origin").transform.GetChild(0).GetChild(0).transform;
            if (_headTransform.CompareTag("MainCamera")) return;
            Debug.LogError("Reference is not the Main Camera");
            _headTransform = transform;
        }

        private void Update()
        {
            // Shoulder to hand direction
            var shouldToHandDir = (transform.position - GetShoulderPos()).normalized;

            // Set rotation from shouldToHandDir
            transform.rotation = Quaternion.LookRotation(shouldToHandDir) * Quaternion.Euler(additionalPitchAngleDeg, 0, 0);
        }

        private Vector3 GetShoulderPos()
        {
            var headPos = _headTransform.position;
            const float yOffset = -0.2f;
            var shoulderPos = new Vector3(headPos.x, headPos.y + yOffset, headPos.z);
            shoulderPos += isRight ? 0.1f * _headTransform.right : -0.1f * _headTransform.right;
            shoulderPos -= 0.1f * Vector3.ProjectOnPlane(_headTransform.forward, Vector3.up).normalized;
            return shoulderPos;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetShoulderPos(), 0.01f);
        }
    }
}
