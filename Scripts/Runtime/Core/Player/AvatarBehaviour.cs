using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    /// <summary>
    /// Manage the avatar's meshes, colors and height.
    /// </summary>
    public class AvatarBehaviour : NetworkBehaviour
    {
        // List of avatar colors
        public List<Material> playerMaterials = new();

        // List of avatar customization
        public List<AvatarDefinition> avatars = new();

        // Used to scale the avatar
        public Transform armature;

        float _avatarScale = 1f;
        SkinnedMeshRenderer _bodyRenderer;

        // Materials used to display in which interaction mode we are using the bracelet.
        public Material interactionModeNoneMaterial;
        public Material interactionModeTeleportMaterial;
        public Material interactionModeRayMaterial;

        RigOrchestrator m_orchestrator;

        void Awake()
        {
            // Cache RigOrchestrator
            if (RigManager.Instance == null)
                Debug.LogError("RigManager not found!");
            else
                m_orchestrator = RigManager.Instance.RigOrchestrator;
        }

        void OnEnable()
        {
            RigManager.Instance.OnHeightCalibrationEnded += ScaleAvatar;
        }

        void OnDisable()
        {
            RigManager.Instance.OnHeightCalibrationEnded -= ScaleAvatar;
        }

        public override void OnNetworkSpawn()
        {
            foreach (AvatarDefinition avatar in avatars)
                avatar.DisableAll();

            InitAvatar();
            m_orchestrator.Visualizer.drawMeshes = false;

            if (IsOwner)
                ScaleAvatar(RigManager.Instance.Height);
            else
                enabled = false;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                m_orchestrator.Visualizer.drawMeshes = true;
        }

        void InitAvatar()
        {
            // Set avatar parts based on clientId
            int partIndex = (int)OwnerClientId;

            int avatarIndex = partIndex % avatars.Count;
            AvatarDefinition avatar = avatars[avatarIndex];

            GameObject head = ActivatePart(partIndex, avatar.heads);
            GameObject hair = ActivatePart(partIndex, avatar.hairs);

            if (IsOwner)
            {
                head.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                hair.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            _bodyRenderer = ActivatePart(partIndex, avatar.bodys).GetComponent<SkinnedMeshRenderer>();

            SetColor(partIndex);

            ActivatePart(partIndex, avatar.pants);
            ActivatePart(partIndex, avatar.shoes);
        }

        GameObject ActivatePart(int id, List<GameObject> parts)
        {
            int partIndex = id % parts.Count;
            parts[partIndex].SetActive(true);
            return parts[partIndex];
        }

        void SetColor(int partIndex)
        {
            Material playerMaterial = playerMaterials[partIndex % playerMaterials.Count];

            Material[] materials = _bodyRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name.Contains("PlayerColor"))
                {
                    materials[i] = playerMaterial;
                    break;
                }
            }

            _bodyRenderer.materials = materials;
        }

        void ScaleAvatar(float height)
        {
            _avatarScale = height / 1.8f; // avatar is normally made for 1.8m
            armature.localScale = Vector3.one * _avatarScale;

            foreach (LimbIK limbIK in GetComponentsInChildren<LimbIK>())
            {
                limbIK.ComputeLength();
            }

            foreach (FootIK footIK in GetComponentsInChildren<FootIK>())
            {
                footIK.ComputeLength();
            }

            AvatarBodySync avatarBodySync = GetComponent<AvatarBodySync>();
            avatarBodySync.SetAvatarScale(_avatarScale);
        }
    }
}