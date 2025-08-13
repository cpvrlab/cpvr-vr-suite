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

    public void UpdateDownloadProgress(string text)
    {
        m_downloadProgressText.text = text;
    }

    public void StartLoadingDisplay()
    {
        m_loadingSpriteTransform.gameObject.SetActive(true);
        StartCoroutine(DisplayLoading());
    }

    public void StopLoadingDisplay()
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
