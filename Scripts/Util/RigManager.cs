using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RigManager : Singleton<RigManager>
{
    [SerializeField] RigOrchestrator m_rigOrchestrator;
    public RigOrchestrator RigOrchestrator { get => m_rigOrchestrator; }
    public UnityEvent enableInteractor;
    public UnityEvent enableTeleport;

    [SerializeField] GameObject m_xrOrigin;
    public GameObject XrOrigin { get => m_xrOrigin; }
    [SerializeField] Image m_fadeImage;

    void Start()
    {
        if (SceneManager.sceneCountInBuildSettings <= 1) return;
        SceneManager.LoadSceneAsync(1);
    }

    public async Task Fade(Color endColor, float duration = 1f)
    {
        var startColor = m_fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            m_fadeImage.color = Color.Lerp(startColor, endColor, t);
            await Task.Yield();
        }
        m_fadeImage.color = endColor;
    }
}
