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

    private void CreateTeleportAreas(Scene activeScene)
    {
        var allGameObjects = FindObjectsOfType<GameObject>();
        var allTeleportObjects = allGameObjects.Where(go => go.scene == activeScene && go.layer == LayerMask.NameToLayer("Teleport")).ToList();
        var allNonTeleportObjects = allGameObjects.Where(go => go.scene == activeScene && go.layer == LayerMask.NameToLayer("Non-Teleport")).ToList();

        var teleportCounter = 0;
        foreach (var go in allTeleportObjects)
        {
            if (!go.TryGetComponent<Collider>(out var _)) go.AddComponent<MeshCollider>();
            if (!go.TryGetComponent<TeleportationArea>(out var _))
            {
                var area = go.AddComponent<TeleportationArea>();
                area.interactionLayers = InteractionLayerMask.GetMask("Teleport");
                teleportCounter++;
            }
        }
        Debug.Log($"{teleportCounter} Teleportation areas added!");

        foreach (var go in allNonTeleportObjects)
        {
            if (!go.TryGetComponent<Collider>(out var _)) go.AddComponent<MeshCollider>();
        }
    }
}
