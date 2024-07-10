using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableOtherCameras : MonoBehaviour
{
    void Awake()
    {
        SceneManager.activeSceneChanged += (_, activeScene) => DisableCameras(activeScene);
    }

    void DisableCameras(Scene activeScene)
    {
        if (RigManager.Instance == null) return;

        var rigCamera = RigManager.Instance.RigOrchestrator.Camera;
        if (!rigCamera.CompareTag("MainCamera")) return;

        var allGameObjects = FindObjectsOfType<GameObject>();
        var allOtherCameras = allGameObjects.Where(
            go => go.scene == activeScene &&
            go.TryGetComponent<Camera>(out var _) &&
            go != rigCamera);

        foreach (var item in allOtherCameras)
            item.SetActive(false);

        Debug.Log($"{allOtherCameras.Count()} Cameras disabled.");
    }
}
