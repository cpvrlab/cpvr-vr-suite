using UnityEngine;

public class InterfaceRayBehaviour : MonoBehaviour
{
    [SerializeField] private bool isRight;
    private Transform _headTransform;
    private Vector3 _directionOffset;

    private void Start()
    {
        _headTransform = GameObject.Find("XR Origin").transform.GetChild(0).GetChild(0).transform;
        if (_headTransform.CompareTag("MainCamera")) return;
        Debug.LogError("Reference is not the Main Camera");
        _headTransform = transform;
    }

    private void Update() => transform.rotation = Quaternion.LookRotation(GetShoulderToHandDirection(GetHeadOffset()));

    private Vector3 GetHeadOffset()
    {
        var headPosition = _headTransform.position;
        const float yOffset = -0.2f;
        var position = new Vector3(headPosition.x, headPosition.y + yOffset, headPosition.z);
        position += isRight ? 0.1f * _headTransform.right : -0.1f * _headTransform.right;
        position -= 0.1f * Vector3.ProjectOnPlane(_headTransform.forward, Vector3.up).normalized;
        return position;
    }

    private Vector3 GetShoulderToHandDirection(Vector3 shoulderPosition) => (transform.position - shoulderPosition).normalized;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetHeadOffset(), 0.01f);
    }
}
