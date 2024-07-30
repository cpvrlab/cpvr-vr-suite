using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    public void ToggleObject(GameObject go) => go.SetActive(!go.activeSelf);
}
