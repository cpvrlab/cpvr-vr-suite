using System;
using System.Collections;
using UnityEngine;

namespace Util {
    /// <summary>
    /// Foot Behaviour using the LimbIK.
    /// </summary>
    public class FootIK : MonoBehaviour {
        // Ratio between Leg Length and the Distance between the foot and the model before moving the foot.
        private const float LegLengthFootMaxDistanceRatio = 0.4f;
        
        // Max Y Angle difference before replacing the foot
        private const float FootMaxAngle = 45;

        // Time taken to replace the foot in second
        private const float StepDuration = .4f;

        // Highest point during a step.
        private const float StepHeight = .1f;

        // Time before placing the feet in a Idle position in milliseconds
        private const long MSBeforeIdle = 1500;
        
        // Max Distance before replacing the foot
        private float _footMaxDistance = .4f;

        // IK target
        public Transform target;
        // Other foot FootIK for knowing if stepping or not
        public FootIK otherFoot;
        // Center of model
        public Transform model;

        // If the foot is currently making a step
        [NonSerialized] public bool Stepping;

        // Parent bone from where the ray will be casted
        private Transform _hipBone;

        // Default hipHeight
        private float _hipHeight;

        // Used for making the foot idle
        private long _lastFootMovement;

        // Used to compute the current body velocity
        private Vector3 _lastModelPosition;
        private Vector3 _velocity;

        private void Start() {
            // Get the bone from where the ray will be cast
            _hipBone = transform.parent.parent;
            
            ComputeLength();
        }

        private void FixedUpdate() {
            // Compute velocity in fixedupdate so the result won't depend on the hardware
            ComputeVelocity();
        }

        // Update is called once per frame
        void Update() {
            // If we are taking a step we don't do any calculation
            if (Stepping) return;

            if (CheckFootDistance())
                return;

            if (CheckFootRotation())
                return;

            Idle();
        }

        /// <summary>
        /// Compute the model velocity.
        /// </summary>
        private void ComputeVelocity() {
            Vector3 modelPosition = model.position;
            
            _velocity = (modelPosition - _lastModelPosition)*30;
            _lastModelPosition = modelPosition;
        }

        /// <summary>
        /// Check if a step is required based on distance.
        /// If the position difference between the foot and the upperbody is bigger than FootMaxDistance we replace the foot.
        /// </summary>
        /// <returns>A bool that says if a footstep has started (true) or not (false).</returns>
        private bool CheckFootDistance() {
            // Vector used to reduce the Y coordinates
            Vector3 flatVector = new Vector3(1, 0, 1);

            Vector3 originPosition = Vector3.Scale(model.position, flatVector);
            Vector3 footPosition = Vector3.Scale(target.position, flatVector);

            float distance = Vector3.Distance(footPosition, originPosition);

            if (distance > _footMaxDistance && !otherFoot.Stepping) {
                StartFootstep();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a step is required based on rotation of upper body.
        /// If the y angle rotation difference between the foot and the upperbody is bigger than FootMaxAngle we replace the foot.
        /// </summary>
        /// <returns>A bool that says if a footstep has started (true) or not (false).</returns>
        private bool CheckFootRotation() {
            float yTarget = target.rotation.eulerAngles.y;
            float ySpine = model.rotation.eulerAngles.y;

            float yDiff = Mathf.DeltaAngle(ySpine, yTarget);

            if (Math.Abs(yDiff) > FootMaxAngle && !otherFoot.Stepping) {
                StartFootstep();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if replacing the foot into a "idle" position is required.
        /// </summary>
        private void Idle() {
            // Compute if the time since last foot movement is bigger than MSBeforeIdle
            long ticksElapsed = DateTime.Now.Ticks - _lastFootMovement;
            long msElapsed = ticksElapsed / 10000;

            if (msElapsed < MSBeforeIdle) return;

            Vector3 targetPosition;

            // Place the foot a bit outside of the body.
            float leftright = Math.Sign(model.InverseTransformPoint(_hipBone.position).x);
            Vector3 direction = Quaternion.AngleAxis(leftright * 5, model.forward) * Vector3.down;

            RaycastHit hit;
            if (Physics.Raycast(_hipBone.position, direction, out hit, _hipHeight+.1f)) {
                targetPosition = hit.point + new Vector3(0, 0.05f, 0);
            } else {
                targetPosition = _hipBone.position + direction * _hipHeight;
            }

            // Do not start a step if we are trying to place the foot at the position is already in
            if (Vector3.Distance(target.position, targetPosition) < .05f) {
                _lastFootMovement = DateTime.Now.Ticks;
                return;
            }

            Stepping = true;

            StartCoroutine(MoveFoot(targetPosition, model.rotation.eulerAngles.y - 15 * leftright));
        }
        
        /// <summary>
        /// Start a footstep sequence.
        /// </summary>
        private void StartFootstep() {
            Stepping = true;

            // Compute the target position based on the velocity.
            float targetRayX = Mathf.Clamp(_velocity.x, -_footMaxDistance, _footMaxDistance);
            float targetRayZ = Mathf.Clamp(_velocity.z, -_footMaxDistance, _footMaxDistance);

            Vector3 targetRay = new Vector3(targetRayX, -_hipHeight, targetRayZ);

            IEnumerator moveFoot;

            // if we hit a raycast we place in on the ground
            // if we don't we place it at a supposed ground height
            RaycastHit hit;
            if (Physics.Raycast(_hipBone.position, targetRay, out hit, _hipHeight*1.5f)) {
                moveFoot = MoveFoot(
                    hit.point + new Vector3(0, 0.05f, 0),
                    model.rotation.eulerAngles.y);
            } else {
                moveFoot = MoveFoot(
                    _hipBone.position + targetRay,
                    model.rotation.eulerAngles.y);
            }

            StartCoroutine(moveFoot);
        }

        /// <summary>
        /// Coroutines to make the foot stepping movement.
        /// </summary>
        /// <param name="finalPosition">Final position of the foot.</param>
        /// <param name="finalAngle">Final Y angle of the foot.</param>
        /// <returns>IEnumerator.</returns>
        private IEnumerator MoveFoot(Vector3 finalPosition, float finalAngle) {
            float timer = 0f;
                
            Vector3 initialPosition = target.position;
            float initialAngle = target.rotation.eulerAngles.y;

            // Lerp the foot from the initial position to the final position
            // and lifting it using LiftFootAnimationCurve
            while (timer < StepDuration) {
                float t = timer / StepDuration;
                Vector3 targetPosition = Vector3.Lerp(initialPosition, finalPosition, t);
                targetPosition += Vector3.up * LiftFootAnimationCurve(t);

                float targetAngle = Mathf.LerpAngle(initialAngle, finalAngle, t);

                Vector3 currentAngles = target.rotation.eulerAngles;

                target.rotation = Quaternion.Euler(currentAngles.x, targetAngle, currentAngles.z);
            
                // Update IK solver
                target.position = targetPosition;

                timer += Time.deltaTime;
                yield return null;
            }

            // Ensure final position is reached
            target.position = finalPosition;

            Stepping = false;
            
            _lastFootMovement =  DateTime.Now.Ticks;
        }

        /// <summary>
        /// Make the lifting of the foot using a function f(x)=y.
        /// </summary>
        /// <param name="x">f(x)</param>
        /// <returns>y</returns>
        private float LiftFootAnimationCurve(float x) {
            return (float)((-Math.Pow(x * 2 - 1, 2) + 1) * StepHeight);
        }

        /// <summary>
        /// Compute the hip height and distance before replacing the foot.
        /// </summary>
        public void ComputeLength() {
            _hipHeight = RigManager.Instance.RigOrchestrator.Origin.InverseTransformPoint(_hipBone.position).y;

            Transform knee = transform.parent;

            float footKneeDistance = Vector3.Distance(transform.position, knee.position);
            float kneeHipDistance = Vector3.Distance(knee.position, _hipBone.position);

            float legLength = footKneeDistance + kneeHipDistance;

            _footMaxDistance = legLength * LegLengthFootMaxDistanceRatio;
        }
    }
}
