using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using Util;
using VR;

enum HeightCalibrationState {
    Waiting,
    Initializing,
    Done
}

namespace V3.Scripts.VR {
    /// <summary>
    /// Manage the avatar's meshes, colors and height.
    /// </summary>
    public class AvatarBehaviour : NetworkBehaviour {
        // List of avatar colors
        public List<Material> playerMaterials = new();

        // List of avatar customization
        public List<AvatarDefinition> avatars = new();

        // Used to scale the avatar
        public Transform armature;

        // Used to calibrate the height
        public AvatarMenuPanel avatarMenuPanel;
        // Calibrate on spawn
        HeightCalibrationState _heightCalibrationState = HeightCalibrationState.Waiting;
        int _frameInState;
        private readonly List<float> _heightData = new();
        private float _avatarScale = 1f;

        private SkinnedMeshRenderer _bodyRenderer;
        
        // Materials used to display in which interaction mode we are using the bracelet.
        public Material interactionModeNoneMaterial;
        public Material interactionModeTeleportMaterial;
        public Material interactionModeRayMaterial;

        public override void OnNetworkSpawn() {      
            // Disable local avatar      
            foreach(AvatarDefinition avatar in avatars) {
                avatar.DisableAll();
            }
            
            // Initialize network avatar
            InitAvatar();

            // Subscribe for interactionMode change to display it through the watch color.
            if(IsOwner) {
                // Disable the hands renderer from XRRig
                XROriginRigReferences.Instance.handVisualizer.drawMeshes = false;
                
                if (RigManager.Instance.RigOrchestrator.TryGetInteractorManager(out HandManager handManager)) {
                    handManager.OnInteractionModeChanged.AddListener(OnInteractionModeChanged);
                }
            } else {
                Destroy(avatarMenuPanel.transform.parent.gameObject);
                enabled = false;
            }
        }

        void FixedUpdate() {
            if(_heightCalibrationState == HeightCalibrationState.Done) return;

            _frameInState++;

            switch(_heightCalibrationState) {
                case HeightCalibrationState.Waiting:
                    if(_frameInState > 50) { // 1 seconds
                        _heightCalibrationState = HeightCalibrationState.Initializing;
                        _frameInState = 0;
                    }
                    break;
                case HeightCalibrationState.Initializing:
                    _heightData.Add(XROriginRigReferences.Instance.Origin.transform.InverseTransformPoint(XROriginRigReferences.Instance.head.position).y + .1f*_avatarScale);
                    if(_frameInState > 250) { // 5 seconds
                        ScaleAvatar();
                        _heightCalibrationState = HeightCalibrationState.Done;
                        _frameInState = 0;
                        _heightData.Clear();
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize the differents avatar's parts
        /// </summary>
        void InitAvatar() {
            // Set avatar parts based on clientId
            int partIndex = (int)OwnerClientId;

            int avatarIndex = partIndex % avatars.Count;
            AvatarDefinition avatar = avatars[avatarIndex];

            GameObject head = ActivatePart(partIndex, avatar.heads);
            GameObject hair = ActivatePart(partIndex, avatar.hairs);
            
            if(IsOwner) {
                head.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                hair.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            _bodyRenderer = ActivatePart(partIndex, avatar.bodys).GetComponent<SkinnedMeshRenderer>();
            
            SetColor(partIndex);

            ActivatePart(partIndex, avatar.pants);
            ActivatePart(partIndex, avatar.shoes);
        }

        GameObject ActivatePart(int id, List<GameObject> parts) {
            int partIndex = id % parts.Count;
            parts[partIndex].SetActive(true);
            return parts[partIndex];
        }

        /// <summary>
        /// Set the shirt's color of the avatar.
        /// </summary>
        /// <param name="partIndex">An int calculated from the OwnerClientId.</param>
        void SetColor(int partIndex) {
            Material playerMaterial = playerMaterials[partIndex % playerMaterials.Count];

            Material[] materials = _bodyRenderer.materials;

            for(int i = 0; i < materials.Length; i++) {
                if(materials[i].name.Contains("PlayerColor")) {
                    materials[i] = playerMaterial;
                    break;
                }
            }

            _bodyRenderer.materials = materials;
        }

        /// <summary>
        /// Scale the avatar based on the height datas gathered.
        /// </summary>
        void ScaleAvatar() {
            float newHeight = _heightData.Average();
            _avatarScale = newHeight / 1.8f; // avatar is normally made for 1.8m
            armature.localScale = Vector3.one * _avatarScale;

            foreach(LimbIK limbIK in GetComponentsInChildren<LimbIK>()) {
                limbIK.ComputeLength();
            }

            foreach(FootIK footIK in GetComponentsInChildren<FootIK>()) {
                footIK.ComputeLength();
            }

            AvatarBodySync avatarBodySync = GetComponent<AvatarBodySync>();
            avatarBodySync.SetAvatarScale(_avatarScale);

            avatarMenuPanel.CalibrationFinished(newHeight);
        }

        /// <summary>
        /// Start the height calibration if it is not already calibrating.
        /// </summary>
        /// <returns>A boolean if the calibration has been started or not.</returns>
        public bool CalibrateHeight() {
            if(_heightCalibrationState != HeightCalibrationState.Done) {
                return false;
            }
            _heightCalibrationState = HeightCalibrationState.Initializing;
            return true;
        }

        /// <summary>
        /// Display the current interaction mode as a color onto the avatar watch
        /// </summary>
        /// <param name="current"></param>
        private void OnInteractionModeChanged(InteractionMode current) {
            if(_bodyRenderer == null) return;

            Material interactionMode = null;
            
            switch(current) {
                case InteractionMode.None:
                    interactionMode = interactionModeNoneMaterial;
                    break;
                case InteractionMode.Teleport:
                    interactionMode = interactionModeTeleportMaterial;
                    break;
                case InteractionMode.Ray:
                    interactionMode = interactionModeRayMaterial;
                    break;
            }

            if (interactionMode == null) return;

            Material[] materials = _bodyRenderer.materials;

            for(int i = 0; i < materials.Length; i++) {
                if (!materials[i].name.Contains("InteractionMode")) continue;
                materials[i] = interactionMode;
                break;
            }

            _bodyRenderer.materials = materials;
        }
    }
}