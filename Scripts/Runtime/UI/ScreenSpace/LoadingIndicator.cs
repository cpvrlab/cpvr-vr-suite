using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingIndicator : MonoBehaviour
{
    [SerializeField] RectTransform m_loadingSpriteTransform;
    [SerializeField] TMP_Text m_downloadProgressText;
    readonly WaitForSecondsRealtime m_wait = new(0.125f);
    readonly Vector3 m_rotationIncrement = new(0, 0, -45);

    Coroutine m_loadingRoutine;
    ushort m_loaders = 0;

    void Start()
    {
        m_loadingSpriteTransform.gameObject.SetActive(false);
        m_downloadProgressText.text = string.Empty;

        if (RigManager.Instance != null)
            RigManager.Instance.Register(this);
    }

    public void StartLoading()
    {
        if (m_loaders <= 0 && m_loadingRoutine == null)
        {
            m_loadingSpriteTransform.gameObject.SetActive(true);
            m_loadingRoutine = StartCoroutine(DisplayLoading());
        }
        
        m_loaders++;
    }

    public void StopLoading()
    {
        m_loaders--;

        if (m_loaders <= 0 && m_loadingRoutine != null)
        {
            StopCoroutine(m_loadingRoutine);
            m_loadingRoutine = null;
            m_loadingSpriteTransform.gameObject.SetActive(false);
            m_downloadProgressText.text = string.Empty;
        }
    }

    public void UpdateText(string text) => m_downloadProgressText.text = text;

    IEnumerator DisplayLoading()
    {
        while (true)
        {
            yield return m_wait;
            m_loadingSpriteTransform.Rotate(m_rotationIncrement);
        }
    }
}
