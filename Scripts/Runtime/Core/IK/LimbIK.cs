using System;
using UnityEngine;

namespace Util
{
    /// <summary>
    /// An IK solution for two limbs / one joint.
    /// </summary>
    public class LimbIK : MonoBehaviour
    {
        // Where the hand should be placed
        public Transform target;

        // Position helper for the flexion of the elbow
        public Transform pole;

        Transform m_elbow;
        Transform m_shoulder;

        // Length of the limbs and the total.
        float _shoulderToElbowLength, _elbowToHandLength, _limbsLength;

        void Start()
        {
            m_elbow = transform.parent;
            m_shoulder = m_elbow.parent;

            ComputeLength();
        }

        void FixedUpdate()
        {
            // Positions of the joints and extremities.
            Vector3 shoulderPosition = m_shoulder.position;
            Vector3 targetPosition = target.position;

            float shoulderToTargetLength = Vector3.Distance(shoulderPosition, targetPosition);

            // Vector between the joints and extremities.
            Vector3 shoulderToTarget = targetPosition - shoulderPosition;
            Vector3 shoulderToPole = pole.position - shoulderPosition;

            // Normal used to make the elbow point towards the pole (stay on the target/shoulder/pole's plane).
            Vector3 armNormal = Vector3.Cross(shoulderToPole, shoulderToTarget);

            m_shoulder.LookAt(targetPosition, armNormal);
            m_shoulder.localRotation *= Quaternion.Euler(90, 0, 0);
            m_shoulder.localRotation *= Quaternion.Euler(0, 90, 0);

            if (shoulderToTargetLength > _limbsLength)
            {
                // If the target is too far for our limbs we just reach as far as possible
                m_elbow.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // Cosine law to calculate the shoulder and the elbow relative up angle.
                float shoulderAngle = -CosineAngle(_shoulderToElbowLength, shoulderToTargetLength, _elbowToHandLength);
                float elbowAngle = -CosineAngle(_shoulderToElbowLength, _elbowToHandLength, shoulderToTargetLength);

                m_shoulder.localRotation *= Quaternion.Euler(shoulderAngle, 0, 0);
                m_elbow.localRotation = Quaternion.Euler(elbowAngle + 180, 0, 0);
            }

            transform.rotation = target.rotation;
        }

        /*
         * 
         *                     /|  
         *                    / |  
         *             adj1  /  |  
         *                  /   | opp
         *                 /    |  
         *       angle -> /_____|  
         *                 adj2   
         * 
         *  Calculate angle from cosine law
         * 
         */
        float CosineAngle(float adj1, float adj2, float opp)
        {
            return (float)Math.Acos((adj1 * adj1 + adj2 * adj2 - opp * opp) / (2 * adj1 * adj2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Compute the length between shoulder/elbow, elbow/wrist and total.
        /// </summary>
        public void ComputeLength()
        {
            if (m_elbow == null || m_shoulder == null) return;

            Vector3 elbowPosition = m_elbow.position;
            Vector3 shoulderPosition = m_shoulder.position;

            _shoulderToElbowLength = Vector3.Distance(shoulderPosition, elbowPosition);
            _elbowToHandLength = Vector3.Distance(elbowPosition, transform.position);
            _limbsLength = _shoulderToElbowLength + _elbowToHandLength;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Vector3 polePosition = pole.position;
            Vector3 shoulderPosition = m_shoulder.position;
            Vector3 targetPosition = target.position;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(polePosition, .01f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(shoulderPosition, polePosition);
            Gizmos.DrawLine(targetPosition, polePosition);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(targetPosition, shoulderPosition);
            Gizmos.color = Color.green;
            Gizmos.DrawLine((targetPosition + shoulderPosition) / 2, polePosition);
        }
    }
}
