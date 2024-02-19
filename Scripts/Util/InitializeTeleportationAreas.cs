using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class InitializeTeleportationAreas : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.activeSceneChanged += (_, activeScene) => CreateTeleportAreas(activeScene);
    }

    public void CreateTeleportAreas(Scene activeScene)
    {
        var allGameObjects = FindObjectsOfType<GameObject>();
        var allTeleportObjects = allGameObjects.Where(go => go.scene == activeScene && 
                                    go.layer == LayerMask.NameToLayer("Teleport")).ToList();
        var allNonTeleportObjects = allGameObjects.Where(go => go.scene == activeScene && 
                                    go.layer == LayerMask.NameToLayer("Non-Teleport")).ToList();

        var teleportAreaCounter = 0;
        foreach (var go in allTeleportObjects)
        {
            if (!go.TryGetComponent<Collider>(out var _))
            {
                Debug.LogWarning($"GameObject {go.name} has no collider attached to it. Teleport will not be available on this object.");
                continue;
            }
            
            if (!go.TryGetComponent<TeleportationArea>(out var _))
            {
                var area = go.AddComponent<TeleportationArea>();
                area.interactionLayers = InteractionLayerMask.GetMask("Teleport");
                area.selectMode = InteractableSelectMode.Multiple;
                teleportAreaCounter++;
            }
        }
        
        Debug.Log($"{allTeleportObjects.Count} Teleport Layer Objects found");
        Debug.Log($"{teleportAreaCounter} Teleport areas added");
        
        foreach (var go in allNonTeleportObjects)
        {
            if (!go.TryGetComponent<Collider>(out var _))
            {
                Debug.LogWarning($"GameObject {go.name} has no collider attached to it. Non-Teleport will not be available on this object.");
                continue;
            }
        }
        Debug.Log($"{allNonTeleportObjects.Count} Non-Teleport Layer Objects found");
    }
}
