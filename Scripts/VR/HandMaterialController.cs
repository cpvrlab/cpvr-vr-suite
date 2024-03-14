using System;
using UnityEngine;

public class HandMaterialController : MonoBehaviour
{
    [SerializeField] Material m_defaultMaterial;
    [SerializeField] Material m_rayMaterial;
    [SerializeField] Material m_teleportMaterial;
    [SerializeField] SkinnedMeshRenderer m_skinnedMeshRenderer;

    public void ChangeHandMaterial(InteractionMode mode)
    {
        if (m_skinnedMeshRenderer == null) return;

        switch (mode)
        {
            case InteractionMode.Ray:
                if (m_rayMaterial != null && m_skinnedMeshRenderer.materials.Length >= 2)
                {
                    var materials = m_skinnedMeshRenderer.materials;
                    materials[1] = m_rayMaterial;
                    m_skinnedMeshRenderer.materials = materials;
                    Debug.Log($"Event received! {mode}");
                }
                break;
            case InteractionMode.Teleport:
                if (m_teleportMaterial != null && m_skinnedMeshRenderer.materials.Length >= 2)
                {
                    var materials = m_skinnedMeshRenderer.materials;
                    materials[1] = m_teleportMaterial;
                    m_skinnedMeshRenderer.materials = materials;
                    Debug.Log($"Event received! {mode}");
                }
                break;
            case InteractionMode.None:
                if (m_defaultMaterial != null && m_skinnedMeshRenderer.materials.Length >= 2)
                {
                    var materials = m_skinnedMeshRenderer.materials;
                    materials[1] = m_defaultMaterial;
                    m_skinnedMeshRenderer.materials = materials;
                    Debug.Log($"Event received! {mode}");
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
