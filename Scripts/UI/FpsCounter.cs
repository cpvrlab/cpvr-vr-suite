using UnityEngine;
using UnityEngine.UI;

namespace cpvrlab_vr_suite.Scripts.UI
{
    public class FpsCounter : MonoBehaviour
    {
        private Text _fpsText;
        private float _deltaTime;

        private void OnEnable() => _deltaTime = 0.0f;

        private void Start() => _fpsText = GetComponent<Text>();

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.01f;
            var currentFPS = (1.0f / _deltaTime).ToString("0.0");
            Debug.Log("FPS: " + currentFPS);
            _fpsText.text = currentFPS + " FPS";
        }
    }
}
