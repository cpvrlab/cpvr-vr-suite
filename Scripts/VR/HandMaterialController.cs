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

        var materials = m_skinnedMeshRenderer.materials;
        switch (mode)
        {
            case InteractionMode.Ray:
                if (m_rayMaterial != null && m_skinnedMeshRenderer.materials.Length >= 2)
                {
                    materials[1] = m_rayMaterial;
                    m_skinnedMeshRenderer.materials = materials;
                }
                break;
            case InteractionMode.Teleport:
                if (m_teleportMaterial != null && m_skinnedMeshRenderer.materials.Length >= 2)
                {
                    materials[1] = m_teleportMaterial;
                    m_skinnedMeshRenderer.materials = materials;
                }
                break;
            case InteractionMode.None:
                if (m_defaultMaterial != null && m_skinnedMeshRenderer.materials.Length >= 2)
                {
                    materials[1] = m_defaultMaterial;
                    m_skinnedMeshRenderer.materials = materials;
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
