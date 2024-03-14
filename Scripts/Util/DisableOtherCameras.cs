using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableOtherCameras : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.activeSceneChanged += (_, activeScene) => DisableCameras(activeScene);
    }

    private void DisableCameras(Scene activeScene)
    {
        if (RigManager.Instance == null) return;

        var rigCamera = RigManager.Instance.XrOrigin.transform.GetChild(0).GetChild(0).GetChild(0);
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
