using System;
using UnityEngine;

public class RayStabilizer : MonoBehaviour
{
    [SerializeField] Transform m_target;
    [SerializeField] Handedness m_handedness;
    [SerializeField][Range(-2f, 1f)] float m_yOffset;
    Transform m_headTransform;
    const float k_xOffset = 0.1f;

    void Start()
    {
        if (RigManager.Instance != null)
            m_headTransform = RigManager.Instance.RigOrchestrator.Camera.transform;
        else
            m_headTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    void Update()
    {
        if (m_target != null)
            transform.position = m_target.position;

        var offset = GetHeadOffset();
        var direction = GetDirection(offset);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void OnEnable()
    {
        if (m_target != null)
            transform.position = m_target.position;
    }

    Vector3 GetHeadOffset()
    {
        var headOffset = m_headTransform.position;
        headOffset.y += m_yOffset;

        switch (m_handedness)
        {
            case Handedness.None:
                break;
            case Handedness.Left:
                headOffset -= k_xOffset * m_headTransform.right;
                break;
            case Handedness.Right:
                headOffset += k_xOffset * m_headTransform.right;
                break;
            default:
                throw new NotImplementedException();
        }

        headOffset -= k_xOffset * Vector3.ProjectOnPlane(m_headTransform.forward, Vector3.up).normalized;

        return headOffset;
    }

    Vector3 GetDirection(Vector3 offset) => (transform.position - offset).normalized;

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetHeadOffset(), 0.01f);
    }

    public enum Handedness
    {
        None,
        Left,
        Right
    }
}