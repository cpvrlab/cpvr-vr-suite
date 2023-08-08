using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Unity.Profiling;

namespace cpvr_vr_suite.Scripts.UI
{
    public class FpsCounter : MonoBehaviour
    {
        private Text _fpsText;
        private float _deltaTime;
        private ProfilerRecorder _drawBatchCountRecorder;
        private ProfilerRecorder _drawCallCountRecorder;
        private ProfilerRecorder _triangleCountRecorder;

        private void OnEnable()
        {
            _deltaTime = 0.0f;
            _drawBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Total Batches Count");
            _drawCallCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            _triangleCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }

        private void OnDisable()
        {
            _drawBatchCountRecorder.Dispose();
            _drawCallCountRecorder.Dispose();
        }


        private void Start() => _fpsText = GetComponent<Text>();

        private void Update()
        {
            var sb = new StringBuilder();
            
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.01f;
            var currentFPS = (1.0f / _deltaTime).ToString("0.0");

            sb.AppendLine($"Draw Calls: {_drawCallCountRecorder.LastValue}");
            sb.AppendLine($"Batches: {_drawBatchCountRecorder.LastValue}");
            sb.AppendLine($"Triangles: {_triangleCountRecorder.LastValue}");
            sb.AppendLine($"FPS: {currentFPS}");
            
            _fpsText.text = sb.ToString();
        }
    }
}
