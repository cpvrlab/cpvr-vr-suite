using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class HandGestureDetector : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.")]
        XRHandTrackingEvents m_handTrackingEvents;

        [SerializeField]
        [Tooltip("The hand shape or pose that must be detected for the gesture to be performed.")]
        ScriptableObject m_handShapeOrPose;

        [SerializeField]
        [Tooltip("The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.")]
        float m_minimumHoldTime = 0.2f;

        [SerializeField]
        [Tooltip("The interval at which the gesture detection is performed.")]
        float m_gestureDetectionInterval = 0.1f;

        [Tooltip("The event fired when the gesture is performed.")]
        public UnityEvent gesturePerformed;

        [Tooltip("The event fired when the gesture is ended.")]
        public UnityEvent gestureEnded;

        XRHandShape m_handShape;
        XRHandPose m_handPose;
        bool m_wasDetected;
        bool m_performedTriggered;
        float m_timeOfLastConditionCheck;
        float m_holdStartTime;

        public Handedness Handedness => m_handTrackingEvents.handedness;

        void OnEnable()
        {
            m_handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

            m_handShape = m_handShapeOrPose as XRHandShape;
            m_handPose = m_handShapeOrPose as XRHandPose;
        }

        void OnDisable()
        {
            m_handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
        }

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_timeOfLastConditionCheck + m_gestureDetectionInterval)
                return;

            var detected =
                m_handTrackingEvents.handIsTracked &&
                m_handShape != null && m_handShape.CheckConditions(eventArgs) ||
                m_handPose != null && m_handPose.CheckConditions(eventArgs);

            if (!m_wasDetected && detected)
            {
                m_holdStartTime = Time.timeSinceLevelLoad;
            }
            else if (m_wasDetected && !detected)
            {
                m_performedTriggered = false;
                gestureEnded?.Invoke();
            }

            m_wasDetected = detected;

            if (!m_performedTriggered && detected)
            {
                var holdTimer = Time.timeSinceLevelLoad - m_holdStartTime;
                if (holdTimer > m_minimumHoldTime)
                {
                    gesturePerformed?.Invoke();
                    m_performedTriggered = true;
                }
            }

            m_timeOfLastConditionCheck = Time.timeSinceLevelLoad;
        }
    }
}