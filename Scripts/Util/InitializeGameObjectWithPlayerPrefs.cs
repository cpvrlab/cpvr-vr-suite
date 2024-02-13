using UnityEngine;

public class InitializeGameObjectWithPlayerPrefs : MonoBehaviour
{
    [SerializeField] string m_playerprefsValue;
    [SerializeField] bool m_defaultValue;

    void Start()
    {
        if (PlayerPrefs.HasKey(m_playerprefsValue))
        {
            int value = PlayerPrefs.GetInt(m_playerprefsValue);
            gameObject.SetActive(value == 1);
        }
        else
        {
            gameObject.SetActive(m_defaultValue);
        }
    }
}
