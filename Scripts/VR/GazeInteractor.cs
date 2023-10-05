using UnityEngine;

public class GazeInteractor : MonoBehaviour
{
    [SerializeField] [Range(1f, 10f)] private float _rayLength;
    public float RayLength
    {
        get => _rayLength <= 0 ? 1 : _rayLength; 
        set => _rayLength = value; 
    }

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _activationTime;
    [SerializeField] private bool _drawGizmos;
    private float _gazeTime = 0f;
    private GazeEvent _gazeInteractable;
    private bool _selectEnterFired = false;

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, _rayLength, _layerMask))
        {
            if (_gazeInteractable == null)
                RegisterInteractable(hit.transform.gameObject);
            else if (hit.transform.gameObject == _gazeInteractable.gameObject &&
                _gazeTime >= _activationTime && 
                !_selectEnterFired)
            {
                _gazeInteractable.OnSelectEnter();
                _selectEnterFired = true;
            }
            else if (_gazeInteractable.gameObject != hit.transform.gameObject)
            {
                UnregisterInteractable();
                RegisterInteractable(hit.transform.gameObject);
            }
            _gazeTime += Time.deltaTime;
        }
        else if (_gazeInteractable != null) 
            UnregisterInteractable();
    }

    private void RegisterInteractable(GameObject interactable)
    {
        if (interactable.TryGetComponent<GazeEvent>(out _gazeInteractable))
            _gazeInteractable.OnHoverEnter();
        else
            Debug.LogError("GameObject is missing a GazeEvent component.");
    }

    private void UnregisterInteractable()
    {
        if (_selectEnterFired) _gazeInteractable.OnSelectExit();
        _gazeInteractable.OnHoverExit();
        _gazeInteractable = null;
        _selectEnterFired = false;
        _gazeTime = 0f;
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _rayLength * transform.forward);
    }
}
