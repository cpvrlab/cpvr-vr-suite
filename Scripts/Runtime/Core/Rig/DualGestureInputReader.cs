using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class DualGestureInputReader : MonoBehaviour, IXRInputButtonReader
    {
        [SerializeField]
        [Tooltip("The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.")]
        XRHandTrackingEvents m_handTrackingEvents;

        [SerializeField]
        [Tooltip("The hand shape or pose that must be detected for the gesture to be performed.")]
        ScriptableObject m_activateHandShapeOrPose;

        [SerializeField]
        [Tooltip("The hand shape or pose that must be detected for the gesture to be released.")]
        ScriptableObject m_deactivateHandShapeOrPose;

        [SerializeField]
        [Tooltip("The interval at which the gesture detection is performed.")]
        float _gestureDetectionInterval = 0.1f;

        XRHandShape m_activationHandShape;
        XRHandPose m_activationHandPose;
        XRHandShape m_deactivationHandShape;
        XRHandPose m_deactivationHandPose;
        float m_timeOfLastConditionCheck;
        bool m_currentlyPerformed, m_wasDetectedThisFrame, m_wasReleasedThisFrame;
        bool m_activateOneFrameFlag, m_deactivateOneFrameFlag;

        void Awake()
        {
            m_activationHandShape = m_activateHandShapeOrPose as XRHandShape;
            m_activationHandPose = m_activateHandShapeOrPose as XRHandPose;
            m_deactivationHandShape = m_deactivateHandShapeOrPose as XRHandShape;
            m_deactivationHandPose = m_deactivateHandShapeOrPose as XRHandPose;
        }

        void Start()
        {
            m_handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
            StartCoroutine(FrameCheck());
        }

        void OnDestroy()
        {
            if (m_handTrackingEvents != null)
                m_handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

            StopCoroutine(FrameCheck());
        }

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_timeOfLastConditionCheck + _gestureDetectionInterval)
                return;

            var activateDetected =
                m_handTrackingEvents.handIsTracked &&
                m_activationHandShape != null && m_activationHandShape.CheckConditions(eventArgs) ||
                m_activationHandPose != null && m_activationHandPose.CheckConditions(eventArgs);

            var deactivateDetected =
                m_handTrackingEvents.handIsTracked &&
                m_deactivationHandShape != null && m_deactivationHandShape.CheckConditions(eventArgs) ||
                m_deactivationHandPose != null && m_deactivationHandPose.CheckConditions(eventArgs);

            if (activateDetected)
                m_currentlyPerformed = true;
            else if (deactivateDetected)
                m_currentlyPerformed = false;

            m_timeOfLastConditionCheck = Time.timeSinceLevelLoad;
        }

        IEnumerator FrameCheck()
        {
            if (!m_activateOneFrameFlag && m_currentlyPerformed && !m_wasDetectedThisFrame)
                m_activateOneFrameFlag = m_wasDetectedThisFrame = true;
            else if (!m_deactivateOneFrameFlag && !m_currentlyPerformed && !m_wasReleasedThisFrame)
                m_deactivateOneFrameFlag = m_wasReleasedThisFrame = true;

            yield return new WaitForEndOfFrame();

            if (m_wasDetectedThisFrame)
                m_wasDetectedThisFrame = false;
            if (m_wasReleasedThisFrame && m_activateOneFrameFlag)
                m_activateOneFrameFlag = m_deactivateOneFrameFlag = m_wasReleasedThisFrame = false;
        }

        public bool ReadIsPerformed() => m_currentlyPerformed;

        public float ReadValue() => m_currentlyPerformed ? 1 : 0;

        public bool ReadWasCompletedThisFrame() => m_wasReleasedThisFrame;

        public bool ReadWasPerformedThisFrame() => m_wasDetectedThisFrame;

        public bool TryReadValue(out float value)
        {
            value = m_currentlyPerformed ? 1 : 0;
            return m_currentlyPerformed;
        }
    }
}
