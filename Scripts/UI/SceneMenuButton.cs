using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMenuButton : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.sceneCountInBuildSettings > 2) return;
        GetComponent<Button>().interactable = false;
    }
}
