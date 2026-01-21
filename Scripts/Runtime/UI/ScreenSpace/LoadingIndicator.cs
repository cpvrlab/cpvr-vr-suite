using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingIndicator : MonoBehaviour
{
    public static LoadingIndicator Instance;
    [SerializeField] RectTransform m_loadingSpriteTransform;
    [SerializeField] TMP_Text m_downloadProgressText;
    readonly WaitForSecondsRealtime m_wait = new(0.125f);
    readonly Vector3 m_rotationIncrement = new(0, 0, -45);

    Coroutine m_loadingRoutine;
    ushort m_loaders = 0;

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

        if (RigManager.Instance != null)
            RigManager.Instance.Register(this);
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

    public void StartLoading() => ShowDisplay();
    public void StopLoading() => HideDisplay();

    void UpdateText(string text) => m_downloadProgressText.text = text;

    void ShowDisplay()
    {
        if (m_loaders <= 0 && m_loadingRoutine == null)
        {
            m_loadingSpriteTransform.gameObject.SetActive(true);
            m_loadingRoutine = StartCoroutine(DisplayLoading());
        }
        
        m_loaders++;
    }

    void HideDisplay()
    {
        m_loaders--;

        if (m_loaders <= 0 && m_loadingRoutine != null)
        {
            StopCoroutine(m_loadingRoutine);
            m_loadingRoutine = null;
            m_loadingSpriteTransform.gameObject.SetActive(false);
            m_downloadProgressText.text = string.Empty;
            //Debug.Log("StopLoadingDisplay");
        }
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
