using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class HandGestureInputReader : MonoBehaviour, IXRInputButtonReader
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

    XRHandShape m_handShape;
    XRHandPose m_handPose;
    bool m_wasDetected;
    bool m_performedTriggered;
    float m_timeOfLastConditionCheck;
    float m_holdStartTime;
    bool m_currentlyPerformed, m_frameCheck;
    BoolReference m_wasDetectedThisFrame, m_wasReleasedThisFrame;

    void Start()
    {
        m_wasDetectedThisFrame = new BoolReference(false);
        m_wasReleasedThisFrame = new BoolReference(false);

        m_handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

        m_handShape = m_handShapeOrPose as XRHandShape;
        m_handPose = m_handShapeOrPose as XRHandPose;
    }

    void Update()
    {
        if (m_currentlyPerformed && !m_frameCheck)
            StartCoroutine(TrueForOneFrame(m_wasDetectedThisFrame));
        else if (!m_currentlyPerformed && m_frameCheck)
            StartCoroutine(TrueForOneFrame(m_wasReleasedThisFrame));

        m_frameCheck = m_currentlyPerformed;
    }

    void OnDestroy()
    {
        if (m_handTrackingEvents != null)
            m_handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (!isActiveAndEnabled || Time.time < m_timeOfLastConditionCheck + m_gestureDetectionInterval)
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
            m_currentlyPerformed = false;
        }

        m_wasDetected = detected;

        if(!m_performedTriggered && detected)
        {
            var holdTimer = Time.timeSinceLevelLoad - m_holdStartTime;
            if(holdTimer > m_minimumHoldTime)
            {
                m_currentlyPerformed = true;
                m_performedTriggered = true;
            }
        }

        m_timeOfLastConditionCheck = Time.time;
    }

    IEnumerator TrueForOneFrame(BoolReference boolRef)
    {
        boolRef.Value = true;
        yield return new WaitForEndOfFrame();
        boolRef.Value = false;
    }

    public bool ReadIsPerformed()
    {
        return m_currentlyPerformed;
    }

    public float ReadValue()
    {
        return m_currentlyPerformed ? 1 : 0;
    }

    public bool ReadWasCompletedThisFrame()
    {
        return m_wasReleasedThisFrame.Value;
    }

    public bool ReadWasPerformedThisFrame()
    {
        return m_wasDetectedThisFrame.Value;
    }

    public bool TryReadValue(out float value)
    {
        value = m_currentlyPerformed ? 1 : 0;
        return m_currentlyPerformed;
    }

    class BoolReference
    {
        public bool Value { get; set; }
        public BoolReference(bool reference)
        {
            Value = reference;
        }
    }
}