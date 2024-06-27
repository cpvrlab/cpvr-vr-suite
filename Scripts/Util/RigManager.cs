using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RigManager : Singleton<RigManager>
{
    public event Action OnHeightCalibrationStarted;
    public event Action<float> OnHeightCalibrationEnded;

    public bool HeightCalculated { get; private set; }
    public float Height { get; private set; }

    [field: SerializeField] public RigOrchestrator RigOrchestrator { get; private set; }
    [SerializeField] Image m_fadeImage;

    bool m_isCalibrating;

    void Start()
    {
        CalibrateHeight();
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

    public async void CalibrateHeight()
    {
        if (m_isCalibrating) return;

        Debug.Log("Started calibration.");
        OnHeightCalibrationStarted?.Invoke();
        m_isCalibrating = true;

        var heightData = new float[75];
        for (int i = 0; i < heightData.Length; i++)
        {
            Debug.Log($"Collected height datapoint nr: {i+1}");
            heightData[i] = RigOrchestrator.Origin.transform.InverseTransformPoint(RigOrchestrator.Camera.transform.position).y + .1f;
            await Task.Delay(10);
        }
        Height = heightData.Average();
        HeightCalculated = true;
        m_isCalibrating = false;

        Debug.Log("Finished calibration.");
        OnHeightCalibrationEnded?.Invoke(Height);
    }
}
