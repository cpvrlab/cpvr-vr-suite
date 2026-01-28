using System;
using Unity.Netcode;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    enum Side
    {
        Left = 1,
        Right = -1
    }

    /// <summary>
    /// Sync the avatar head and hands position/rotation and computes the body's position afterward.
    /// </summary>
    public class AvatarBodySync : NetworkBehaviour
    {
        [Header("Network Position/Rotation Synchronisation")]
        // Head bone transform to sync with VR
        public Transform headBone;

        // Main bone to move the avatar
        public Transform pelvisBone;

        // For upperbody leaning
        public Transform upperBodyBone;

        // Wrist bones to sync with VR
        public Transform leftHandRootBone;
        public Transform rightHandRootBone;

        // IK targets for the arms
        public Transform leftHandTarget;
        public Transform rightHandTarget;

        // Used to offset the avatar so the eyes position of the avatar matches the player's eyes position
        public Vector3 headsetOffset;

        Vector3 m_headToMainBone;
        float m_avatarScale = 1f;

        // Boolean used when the body is rotating towards the head y rotation
        bool _rewind;

        RigOrchestrator m_rigOrchestrator;

        void Start()
        {
            m_headToMainBone = pelvisBone.position - headBone.position;

            if (RigManager.Instance == null)
                Debug.LogError("RigManager not found!");
            else
                m_rigOrchestrator = RigManager.Instance.Get<RigOrchestrator>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                /*
                    Detach the hands bone from the parent so the maximum precision can be made  since it is the most important part and
                    then the IK will try to make the connection between the wrist and the hands as seemless as possible.
                */
                Transform avatarTransform = transform;

                leftHandRootBone.parent = avatarTransform;
                rightHandRootBone.parent = avatarTransform;
            }
            else
            {
                // Disable the IK scripts since the coordinates of the limbs are sent via network.
                LimbIK[] limbScripts = GetComponentsInChildren<LimbIK>();
                FootIK[] footScripts = GetComponentsInChildren<FootIK>();

                foreach (LimbIK ikScript in limbScripts)
                    ikScript.enabled = false;
                foreach (FootIK ikScript in footScripts)
                    ikScript.enabled = false;

                Debug.Log("Disabled " + limbScripts.Length + " LimbIK scripts.");
                Debug.Log("Disabled " + footScripts.Length + " FootIK scripts.");

                enabled = false;
            }
        }

        void Update()
        {
            // Remove this check for non-network
            if (!IsOwner) return;

            SyncHead();

            BodyIK();

            SyncHands();
        }

        /// <summary>
        /// Syncronize the headset position and rotation with the avatar head.
        /// </summary>
        void SyncHead()
        {
            if (m_rigOrchestrator == null) return;

            var vrHead = m_rigOrchestrator.Camera.transform;

            var scaledHeadsetOffset = headsetOffset * m_avatarScale;
            var nextHeadPos = vrHead.position + vrHead.up * scaledHeadsetOffset.y + vrHead.right * scaledHeadsetOffset.x + vrHead.forward * scaledHeadsetOffset.z;
            var nextHeadRot = vrHead.rotation * Quaternion.Euler(0, 180, 0);

            headBone.SetPositionAndRotation(nextHeadPos, nextHeadRot);
        }

        /// <summary>
        /// Compute the body position based on the head.
        /// </summary>
        void BodyIK()
        {
            // Since the head is a child of the main bone make it stay at the same place during the mainBone movement
            Vector3 keepPosition = headBone.position;
            Quaternion keepRotation = headBone.rotation;

            // if we are leaning forwards
            float xHead = headBone.localRotation.eulerAngles.x;
            float leaningValue = 0f;

            float upperBodyLeaningX = 0f;

            if (xHead is < 360 and > 270)
            {
                leaningValue = -xHead + 360;

                upperBodyLeaningX = 90 - leaningValue;
                upperBodyLeaningX = 90 - upperBodyLeaningX;

                leaningValue /= 90;
            }

            upperBodyBone.localRotation = Quaternion.Euler(-upperBodyLeaningX / 3, 0, 0);

            // y rotation difference between the head and the model
            float yHead = headBone.rotation.eulerAngles.y;
            float yMainBone = pelvisBone.rotation.eulerAngles.y;
            float yDiff = Mathf.DeltaAngle(yHead, yMainBone);

            Vector3 headToMainBoneUpdated = Vector3.Scale(m_headToMainBone * m_avatarScale, new Vector3(1, 1, 10 * leaningValue));

            Vector3 headToMainWithYRotation = Quaternion.AngleAxis(yHead - 180, Vector3.up) * headToMainBoneUpdated;

            Vector3 moveVector = headBone.position + headToMainWithYRotation - pelvisBone.position;

            // Move the body towards the head.
            // The farthest the body is, faster it will go.
            if (moveVector.magnitude > .4)
            {
                // If the body is way to far than just put it in place.
                pelvisBone.position += moveVector;
            }
            else
            {
                float bonus = 1;

                // if we are going backward
                if (pelvisBone.InverseTransformPoint(pelvisBone.position + moveVector).z > 0)
                {
                    bonus *= 4;
                }

                Vector3 pelvisPosition = pelvisBone.position;
                pelvisPosition = Vector3.MoveTowards(pelvisPosition, pelvisPosition + moveVector, Time.deltaTime * (moveVector.magnitude * 10) * bonus);
                pelvisBone.position = pelvisPosition;
            }

            // Body Y rotation based on the head/body diff rotation
            if (!_rewind && Math.Abs(yDiff) > 15)
            {
                _rewind = true;
            }
            if (_rewind && Math.Abs(yDiff) < 2)
            {
                _rewind = false;
            }
            if (_rewind)
            {
                pelvisBone.Rotate(Vector3.up, -yDiff * 2 * Time.deltaTime, Space.World);
            }

            // Constraint on the head transform
            headBone.position = keepPosition;
            headBone.rotation = keepRotation;
        }

        /// <summary>
        /// Synchronize the hand's bones from the XRRig to the avatar.
        /// </summary>
        void SyncHands()
        {
            if (m_rigOrchestrator == null) return;

            // Sync the root bone of the hand
            if (m_rigOrchestrator.LeftHandInteractorManager.TryGetHandPosition(out var leftHandTransform))
                SyncTransform(leftHandRootBone, leftHandTransform, true, Side.Left);
            else
                leftHandRootBone.SetPositionAndRotation(pelvisBone.position + 0.2f * pelvisBone.right, pelvisBone.rotation * Quaternion.Euler(new Vector3(180f, 0, 0)));

            if (m_rigOrchestrator.RightHandInteractorManager.TryGetHandPosition(out var rightHandTransform))
                SyncTransform(rightHandRootBone, rightHandTransform, true, Side.Right);
            else
                rightHandRootBone.SetPositionAndRotation(pelvisBone.position + -0.2f * pelvisBone.right, pelvisBone.rotation * Quaternion.Euler(new Vector3(180f, 0f, 0f)));

            // Sync the ik target transform with the root bone
            SyncTransform(leftHandTarget, leftHandRootBone, false, Side.Left);
            SyncTransform(rightHandTarget, rightHandRootBone, false, Side.Right);

            // Start the recursive synchronization of the hand's bones
            // Since the fingers position in the tree can be different between the model the avatar is using and the 
            // OpenXR hands, we sync them using their names. thumb, index, middle, ring, little
            if (m_rigOrchestrator.LeftHandInteractorManager.HandTrackingEvents.handIsTracked)
            {
                for (int i = 0; i < leftHandRootBone.childCount; i++)
                {
                    string fingerName = leftHandRootBone.GetChild(i).name.Split('_')[0];
                    SyncBoneRec(leftHandRootBone.GetChild(i), FindBoneByName(m_rigOrchestrator.LeftHandInteractorManager.HandMeshRenderer.rootBone, fingerName), Side.Left);
                }
            }

            if (m_rigOrchestrator.RightHandInteractorManager.HandTrackingEvents.handIsTracked)
            {
                for (int i = 0; i < rightHandRootBone.childCount; i++)
                {
                    string fingerName = rightHandRootBone.GetChild(i).name.Split('_')[0];
                    SyncBoneRec(rightHandRootBone.GetChild(i), FindBoneByName(m_rigOrchestrator.RightHandInteractorManager.HandMeshRenderer.rootBone, fingerName), Side.Right);
                }
            }
        }

        /// <summary>
        /// Find a bone inside a parent transform based on a name.
        /// </summary>
        /// <param name="parent">Transform parent of the supposed bone.</param>
        /// <param name="fingerName">The string that the child name should contains.</param>
        /// <returns>Transform of the child if found.</returns>
        /// <exception cref="Exception">If the name can't be found throw an exception.</exception>
        Transform FindBoneByName(Transform parent, string fingerName)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).name.ToLower().Contains(fingerName.ToLower()))
                    return parent.GetChild(i).transform;
            }
            throw new Exception("Bone child not found!");
        }

        /// <summary>
        /// Syncronize the avatar bone based on the same bone on the XRRig.
        /// </summary>
        /// <param name="networkBone">Transform of the bone to sync.</param>
        /// <param name="vrBone">Transform of the bone to get the data from.</param>
        /// <param name="side">Side of the body to correct the rotation if needed.</param>
        void SyncBoneRec(Transform networkBone, Transform vrBone, Side side)
        {
            SyncTransform(networkBone, vrBone, true, side);

            for (int i = 0; i < networkBone.childCount; i++)
            {
                if (networkBone.GetChild(i).name.Contains("end")) continue;
                if (vrBone.GetChild(i).name.Contains("VelocityPrefab")) continue;

                SyncBoneRec(networkBone.GetChild(i), vrBone.GetChild(i), side);
            }
        }

        /// <summary>
        /// Syncronize the rotation and position of two transform.
        /// </summary>
        /// <param name="to">Transform to sync.</param>
        /// <param name="from">Transform to get the data from.</param>
        /// <param name="correct">If a correction from right-handed(blender) to left-handed(unity) coordinate system has to be done.</param>
        /// <param name="side">Side of the body to correct the rotation if needed.</param>
        void SyncTransform(Transform to, Transform from, bool correct, Side side)
        {
            to.SetPositionAndRotation(from.position, from.rotation);

            if (!correct) return;

            to.rotation *= Quaternion.Euler(90, 0, 0);
            to.Rotate(0, 90 * (float)side, 0, Space.Self);
        }

        public void SetAvatarScale(float scale)
        {
            m_avatarScale = scale;
        }
    }
}
