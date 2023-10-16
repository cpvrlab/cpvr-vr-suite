using UnityEngine;

public class InitializeGameObjectWithPlayerPrefs : MonoBehaviour
{
    [SerializeField] private string _playerprefsValue;

    void Start()
    {
        int value = PlayerPrefs.GetInt(_playerprefsValue);
        gameObject.SetActive(value == 1);
    }
}
