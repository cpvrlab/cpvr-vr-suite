using UnityEngine;
using UnityEngine.InputSystem;

namespace cpvr_vr_suite.Scripts.VR
{
    public class Teleporting : MonoBehaviour
    {
        [Header("Ray settings")]
        [SerializeField] private LayerMask teleportLayer;
        [SerializeField] private float length;                        // How far will the arc be
        [SerializeField, Range(1, 7)] private byte resolutionLevel;  
        [SerializeField] private float deepestPoint;                    // Deepest point on the map
        [SerializeField] private Color validColor;               
        [SerializeField] private Color invalidColor;             
        [SerializeField] private float lineThickness = 0.01f;    
        [SerializeField] private Material lineMaterial;
        [SerializeField] private int resolution;
        [Header("Object references")]
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private Transform headTransform;
        [SerializeField] private Transform xrOrigin;
        [Header("Action reference")]
        [SerializeField] private InputActionProperty teleportAction;

        private Vector3 _teleportPosition;           
        private Transform _circle;
        private LineRenderer _lineTeleport;     
        private Vector3[] _lineTeleportPoints;  
        private bool _teleport;

        private void Awake()
        {
            resolution           = 1 << resolutionLevel;
            _lineTeleport        = gameObject.AddComponent<LineRenderer>();
            _lineTeleportPoints  = new Vector3[resolution + 1];

            _circle = Instantiate(circlePrefab, xrOrigin.parent).transform;
            _circle.GetComponent<Renderer>().sharedMaterial.color = validColor;

            if (rayOrigin == null)
                rayOrigin = transform;

            SetupLineRenderer();
            EnableAll(false);
        }

        private void FixedUpdate()
        {
            EnableAll(_teleport);

            if (!_teleport) return;
            DrawTeleportArc();
        }

        private void OnEnable()
        {
            teleportAction.action.performed += _ => _teleport = true;
            teleportAction.action.canceled += _ => DisableTeleport();
        }
        
        private void OnDisable()
        {
            _teleport = false;
            _lineTeleport.enabled = false;
            _circle.gameObject.SetActive(false);
        }

        private void DrawTeleportArc()
        {
            var handPos = rayOrigin.position;
            var handRot = rayOrigin.rotation;
            var handAngleXRad = -handRot.eulerAngles.x * Mathf.Deg2Rad;
            var handAngleYRad = (-handRot.eulerAngles.y + 90.0f) * Mathf.Deg2Rad;
            var heightFromDeepestPoint = handPos.y - deepestPoint;

            // Calculate arc in physics, check if a teleport area was found and draw the arc
            CreateArc(heightFromDeepestPoint, length, handAngleXRad, handAngleYRad, handPos, _lineTeleportPoints);
            CastRay(_lineTeleportPoints, out var lastPoint, out var hitPos, out var hitNormal, out var valid);
            DrawTeleport(hitPos, hitNormal, lastPoint, valid);

            // Store the found valid position 
            _teleportPosition = valid ? hitPos : Vector3.zero;
        }

        private void DisableTeleport()
        {
            if (!_teleport) return;
            _teleport = false;
            if (_teleportPosition == Vector3.zero /*|| PalmFacesHead()*/) return;   // Disabled due to bug with inputsystem
            // Add camera to origin offset to teleportPosition
            var originPosition = xrOrigin.position;
            var headPosition = headTransform.position;
            var deltaPosition = originPosition - new Vector3(headPosition.x, originPosition.y, headPosition.z);
            _teleportPosition += deltaPosition;
            xrOrigin.position = _teleportPosition;
            _teleportPosition = Vector3.zero;
        }

        private void DrawTeleport(Vector3 hitPos, Vector3 hitNormal, int lastPoint, bool valid)
        {
            var hasHit = hitPos != Vector3.zero && valid; // If something was hit
            _lineTeleport.positionCount = hasHit ? lastPoint + 1 : _lineTeleportPoints.Length;
            _lineTeleport.material.color = hasHit ? validColor : invalidColor;
            var maxIteration = hasHit ? lastPoint : _lineTeleportPoints.Length;

            EnableCircle(hasHit);

            for (var i = 0; i < maxIteration; i++)
                _lineTeleport.SetPosition(i, _lineTeleportPoints[i]);

            if (!hasHit) return;
            _lineTeleport.SetPosition(lastPoint, hitPos);
            SetCircle(hitPos, hitNormal);
        }

        // Sets the position and rotation of the circle
        private void SetCircle(Vector3 pos, Vector3 normal) => _circle.position = pos + normal * 0.01f;

        // Enables/Disables the lineTeleport and Circle (Visible Objects)
        private void EnableAll(bool isEnabled)
        {
            _lineTeleport.enabled = isEnabled;
            EnableCircle(isEnabled);
        }

        // Enables/Disables Circle GameObject
        private void EnableCircle(bool isEnabled) => _circle.gameObject.SetActive(isEnabled);

        // Calculates an arc using 3D projectile motion, returns points in WS
        private void CreateArc(float deepest,
            float velo,
            float angX,
            float angY,
            Vector3 controllerPos,
            Vector3[] p)
        {
            var cosX = Mathf.Cos(angX);
            var sinX = Mathf.Sin(angX);
            var cosY = Mathf.Cos(angY);
            var sinY = Mathf.Sin(angY);
            var height = deepest + Mathf.Pow(velo, 2) * sinX * sinX / (2.0f * 9.81f);
            var totalTime = velo * sinX / 9.81f + Mathf.Sqrt(height * 2.0f / 9.81f);
            var partTime = totalTime / resolution;
            var time = partTime;
            p[0] = controllerPos;   // First position is the origin of the arc

            for (var i = 1; i < (resolution + 1); i++)
            {
                Vector3 arcPos = new()
                {
                    x = velo * cosX * cosY * time,
                    y = velo * sinX * time - 0.5f * 9.81f * Mathf.Pow(time, 2),
                    z = velo * cosX * sinY * time
                };
                arcPos += controllerPos; // Adds position of the controller e.g. the origin of the arc
                p[i] = arcPos;
                time += partTime;
            }
        }

        // Checks if a teleport area is in the way of the arc. Shoots a ray every second calculated position
        private void CastRay(Vector3[] p, out int lastPoint, out Vector3 hitPos, out Vector3 hitNormal, out bool valid)
        {
            lastPoint = 0;
            hitPos = Vector3.zero;
            hitNormal = Vector3.zero;
            valid = false;

            var length = p.Length - 1;
            var i = 0;
            var hit = new RaycastHit();

            while (i < length)
            {
                if (!Physics.Linecast(p[i], p[i + 2], out hit))
                    i += 2;
                else
                {
                    lastPoint = i + 1;
                    break;
                }
            }

            hitPos = hit.point;
            hitNormal = hit.normal;
            if (hit.collider != null)
                valid = ((1 << hit.collider.gameObject.layer) & teleportLayer) != 0;
            // Needed to specify preciser which ray was the last
            if (i >= length) return;
            if (!Physics.Linecast(p[i], (p[i + 1]), out hit)) return;
            lastPoint = i;
            hitPos = hit.point;
            hitNormal = hit.normal;
            valid = ((1 << hit.collider.gameObject.layer) & teleportLayer) != 0 /*&& !PalmFacesHead()*/;    //Disabled due to bug
        }
        
        private bool PalmFacesHead() => Vector3.Dot(headTransform.forward, transform.up) > 0.5f;

        private void SetupLineRenderer()
        {
            _lineTeleport.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineTeleport.receiveShadows = false;
            _lineTeleport.allowOcclusionWhenDynamic = false;
            _lineTeleport.loop = false;
            _lineTeleport.material = lineMaterial;
            _lineTeleport.material.color = validColor;
            _lineTeleport.startWidth = lineThickness;
            _lineTeleport.endWidth = lineThickness;
        }
    }
}