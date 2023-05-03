using UnityEngine;
using UnityEngine.UI;

namespace cpvrlab_vr_suite.Scripts
{
    public class FpsCounter : MonoBehaviour
    {
        private Text _fpsText;
        private void Start() => _fpsText = GetComponent<Text>();

        private void Update()
        {
            float currentFPS = (int)(1f / Time.deltaTime);
            _fpsText.text = currentFPS.ToString("0.00") + " FPS";
        }
    }
}
