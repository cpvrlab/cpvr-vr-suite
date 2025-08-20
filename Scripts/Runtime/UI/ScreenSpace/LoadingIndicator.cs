using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingIndicator : MonoBehaviour
{
    public static LoadingIndicator Instance;
    [SerializeField] RectTransform m_loadingSpriteTransform;
    [SerializeField] TMP_Text m_downloadProgressText;
    readonly WaitForSecondsRealtime m_wait = new(0.125f);
    readonly Vector3 m_rotationIncrement = new Vector3(0, 0, -45);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        m_loadingSpriteTransform.gameObject.SetActive(false);
        m_downloadProgressText.text = string.Empty;
    }

    public static void StartLoadingDisplay()
    {
        if (Instance != null)
            Instance.ShowDisplay();
    }

    public static void StopLoadingDisplay()
    {
        if (Instance != null)
            Instance.HideDisplay();
    }

    public static void UpdateDownloadProgress(string text)
    {
        if (Instance != null)
            Instance.UpdateText(text);
    }

    void UpdateText(string text) => m_downloadProgressText.text = text;

    void ShowDisplay()
    {
        m_loadingSpriteTransform.gameObject.SetActive(true);
        StartCoroutine(DisplayLoading());
    }

    void HideDisplay()
    {
        StopCoroutine(DisplayLoading());
        m_loadingSpriteTransform.gameObject.SetActive(false);
        m_downloadProgressText.text = string.Empty;
        //Debug.Log("StopLoadingDisplay");
    }

    IEnumerator DisplayLoading()
    {
        while (true)
        {
            yield return m_wait;
            m_loadingSpriteTransform.Rotate(m_rotationIncrement);
        }
    }
}
