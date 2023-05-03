using UnityEngine;
using UnityEngine.InputSystem;

namespace cpvrlab_vr_suite.Scripts
{
    public class Teleporting : MonoBehaviour
    {
        [SerializeField] private LayerMask teleportLayer;
        [SerializeField] private float length;                        // How far will the arc be
        [SerializeField, Range(1, 7)] private byte resolutionLevel;  
        [SerializeField] private float deepestPoint;                    // Deepest point on the map
        [SerializeField] private Color validColor;               
        [SerializeField] private Color invalidColor;             
        [SerializeField] private float lineThickness = 0.01f;    
        [SerializeField] private Material lineMaterial;
        [SerializeField] private int resolution;
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private Transform headTransform;
        [SerializeField] private InputActionProperty leftHandPosition;
        [SerializeField] private InputActionProperty leftHandRotation;
        [SerializeField] private InputActionProperty rightHandPosition;
        [SerializeField] private InputActionProperty rightHandRotation;

        private Vector3 _teleportPosition;           
        private Transform _circle;
        private LineRenderer _lineTeleport;     
        private Vector3[] _lineTeleportPoints;  
        private bool _teleport;
        private bool _rightHand;

        private void Awake()
        {
            resolution          = 1 << resolutionLevel;
            _lineTeleport        = gameObject.AddComponent<LineRenderer>();
            _lineTeleportPoints  = new Vector3[resolution + 1];

            _circle = Instantiate(circlePrefab, transform.parent).transform;
            _circle.GetComponent<Renderer>().sharedMaterial.color = validColor;

            SetupLineRenderer();
            EnableAll(false);
        }

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

        private void FixedUpdate()
        {
            EnableAll(_teleport);

            if (!_teleport) return;
            DrawTeleportArc();
        }

        private void DrawTeleportArc()
        {
            var handPosition = _rightHand ? GetRightHandPosition() : GetLeftHandPosition();
            var handRotation = _rightHand ? GetRightHandRotation() : GetLeftHandRotation();
            var controllerPos = CalculatePositionInWorldSpace(handPosition);

            GetHandAngleInWorldSpace(handRotation, out var angleXRad, out var angleYRad);

            var heightFromDeepestPoint = controllerPos.y - deepestPoint;

            // Calculate arc in physics, check if a teleportable area was found and draw the arc
            CreateArc(heightFromDeepestPoint, resolution, length, angleXRad, angleYRad, controllerPos, _lineTeleportPoints);
            CastRay(_lineTeleportPoints, out var lastPoint, out var hitPos, out var hitNormal, out var valid);
            DrawTeleport(hitPos, hitNormal, lastPoint, valid);

            // Store the found valid position 
        
            _teleportPosition = valid ? hitPos : Vector3.zero;
        }

        public void Teleport(bool rightHand)
        {
            _teleport = true;
            _rightHand = rightHand;
        }

        public void DisableTeleport()
        {
            CancelTeleport();
            if (_teleportPosition == Vector3.zero) return;
            // Add camera to origin offset to teleportPosition
            var originPosition = transform.position;
            var headPosition = headTransform.position;
            var deltaPosition = originPosition - new Vector3(headPosition.x, originPosition.y, headPosition.z);
            _teleportPosition += deltaPosition;
            transform.position = _teleportPosition;
            _teleportPosition = Vector3.zero;
        }

        public void CancelTeleport() => _teleport = false;

        private Vector3 GetLeftHandPosition() => leftHandPosition.action.ReadValue<Vector3>();

        private Quaternion GetLeftHandRotation() => leftHandRotation.action.ReadValue<Quaternion>();

        private Vector3 GetRightHandPosition() => rightHandPosition.action.ReadValue<Vector3>();

        private Quaternion GetRightHandRotation() => rightHandRotation.action.ReadValue<Quaternion>();

        // stores angleX_RAD and angleY_RAD of the hand rotation in WS
        private void GetHandAngleInWorldSpace(Quaternion handRotation, out float angleXRad, out float angleYRad)
        {
            var handRotationEuler = (transform.rotation * handRotation).eulerAngles;
            angleXRad = -handRotationEuler.x * Mathf.Deg2Rad;
            angleYRad = (-handRotationEuler.y + 90.0f) * Mathf.Deg2Rad;
        }

        // calculates the controller or hand position in WS
        private Vector3 CalculatePositionInWorldSpace(Vector3 posOS) => 
            Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * posOS + transform.position;
    
        private void DrawTeleport(Vector3 hitPos, Vector3 hitNormal, int lastPoint, bool valid)
        {
            var hasHit = hitPos != Vector3.zero && valid; // If something was hit
            _lineTeleport.positionCount = (hasHit) ? lastPoint + 1 : _lineTeleportPoints.Length;
            _lineTeleport.material.color = (hasHit) ? validColor : invalidColor;
            var maxIteration = (hasHit) ? lastPoint : _lineTeleportPoints.Length;

            EnableCircle(hasHit);

            for (var i = 0; i < maxIteration; i++)
                _lineTeleport.SetPosition(i, _lineTeleportPoints[i]);

            if (!hasHit) return;
            _lineTeleport.SetPosition(lastPoint, hitPos);
            SetCircle(hitPos, hitNormal);
        }

        // Sets the position and rotation of the circle
        private void SetCircle(Vector3 pos, Vector3 normal)
        {
            if (_circle != null) _circle.transform.position = pos + normal * 0.01f;
        }

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
            int res,
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
            var height = deepest + (velo * velo * sinX * sinX) / 19.62f;
            var totalTime = (velo * sinX) / 9.81f + Mathf.Sqrt(height * 2.0f / 9.81f);
            var partTime = totalTime / res;
            var time = partTime;
            p[0] = controllerPos;   // First position is the origin of the arc

            for (var i = 1; i < (res + 1); i++)
            {
                p[i].x = velo * cosX * cosY * time;
                p[i].y = velo * sinX * time - 0.5f * 9.81f * time * time;
                p[i].z = velo * cosX * sinY * time;
                p[i] += controllerPos;  // Adds position of the controller e.g. the origin of the arc
                time += partTime;
            }
        }

        // Checks if a teleportable area is in the way of the arc. Shoots a ray every second calculated position
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
            if (i < length)
            {
                if (Physics.Linecast(p[i], (p[i + 1]), out hit))
                {
                    lastPoint = i;
                    hitPos = hit.point;
                    hitNormal = hit.normal;
                    valid = ((1 << hit.collider.gameObject.layer) & teleportLayer) != 0;
                }
            }
        }
    }
}