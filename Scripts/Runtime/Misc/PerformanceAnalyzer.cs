using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cpvr_vr_suite.Scripts.Runtime.Util;
using Unity.Profiling;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    [Serializable]
    public struct Position
    {
        public Transform transform;
        public string name;
    }

    public class PerformanceAnalyzer : MonoBehaviour
    {
        [SerializeField] private string emailAddress;
        [SerializeField] private int profilerCapacity = 100;
        [SerializeField][Range(0.5f, 5f)] private float recordIntervall = 1f;
        [SerializeField] private List<Position> positions;
        ProfilerRecorder m_mainThreadTimeRecorder;
        ProfilerRecorder m_batchCountRecorder;
        ProfilerRecorder m_triangleCountRecorder;
        ProfilerRecorder m_drawCallCountRecorder;
        Transform m_camera;

        void Awake()
        {
            m_mainThreadTimeRecorder = new ProfilerRecorder(ProfilerCategory.Internal, "Main Thread", profilerCapacity);
            m_batchCountRecorder = new ProfilerRecorder(ProfilerCategory.Render, "Total Batches Count", profilerCapacity);
            m_triangleCountRecorder = new ProfilerRecorder(ProfilerCategory.Render, "Triangles Count", profilerCapacity);
            m_drawCallCountRecorder = new ProfilerRecorder(ProfilerCategory.Render, "Draw Calls Count", profilerCapacity);
        }

        void Start()
        {
            m_camera = transform.GetChild(0);
            MeasureAllPositions();
        }

        async void MeasureAllPositions()
        {
            if (positions.Count == 0 || m_camera == null || emailAddress.Equals("")) return;

            var reportBuilder = new StringBuilder();
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HH-mm");

            reportBuilder.AppendLine($"# Performance Report {timestamp}:");
            reportBuilder.AppendLine("**Measuring the following metrics:**");
            reportBuilder.AppendLine("- Frame time (FT)");
            reportBuilder.AppendLine("- Batches count (B)");
            reportBuilder.AppendLine("- Triangles count (T)");
            reportBuilder.AppendLine("- Draw calls (DC)");
            reportBuilder.AppendLine();
            reportBuilder.AppendLine("| Position | FPS | FT AVG (ms) | FT MIN (ms) | FT MAX (ms) | B AVG | B MIN | B MAX | T AVG | T MIN | T MAX | DC AVG | DC MIN | DC MAX |");
            reportBuilder.AppendLine("|----------|-----|-------------|-------------|-------------|-------|-------|-------|-------|-------|-------|--------|--------|--------|");

            await Task.Delay(1000);

            foreach (var element in positions)
            {
                var entry = await MeasurePosition(element);
                reportBuilder.AppendLine(entry);
            }

            var report = reportBuilder.ToString();

            var filePath = Path.Combine(Application.persistentDataPath, $"Performance_Report-{timestamp}.md");
            File.WriteAllText(filePath, report);

            Debug.Log($"Summary report saved to: {filePath}");

            if (await MailSender.SendEmail(emailAddress, $"Performance_Report-{timestamp}", "", filePath))
            {
                Debug.Log($"Summary report sent to: {emailAddress}");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }

        }

        async Task<string> MeasurePosition(Position pos)
        {
            var rep = new StringBuilder();
            m_camera.SetPositionAndRotation(pos.transform.position, pos.transform.rotation);
            StartRecorders();
            await Task.Delay((int)Mathf.Round(recordIntervall * 1000));
            StopRecorders();
            var mtValues = GetRecorderData(m_mainThreadTimeRecorder);
            var bValues = GetRecorderData(m_batchCountRecorder);
            var tValues = GetRecorderData(m_triangleCountRecorder);
            var dcValues = GetRecorderData(m_drawCallCountRecorder);

            var entry = $"| {pos.name} | {1000 / (Mathf.Round((float)mtValues.Average()) * 1e-6f):F1} | {Mathf.Round((float)mtValues.Average()) * 1e-6f:F1} | {mtValues.Min() * 1e-6f:F1} | {mtValues.Max() * 1e-6f:F1} |" +
                        $" {Mathf.Round((float)bValues.Average())} | {bValues.Min()} | {bValues.Max()} |" +
                        $" {Mathf.Round((float)tValues.Average())} | {tValues.Min()} | {tValues.Max()} |" +
                        $" {Mathf.Round((float)dcValues.Average())} | {dcValues.Min()} | {dcValues.Max()} |";

            ResetRecorders();
            return entry;
        }

        List<long> GetRecorderData(ProfilerRecorder recorder)
        {
            var data = new List<ProfilerRecorderSample>();
            recorder.CopyTo(data);
            var values = data.Select(p => p.Value).ToList();
            return values;
        }

        void StartRecorders()
        {
            m_mainThreadTimeRecorder.Start();
            m_batchCountRecorder.Start();
            m_triangleCountRecorder.Start();
            m_drawCallCountRecorder.Start();
        }

        void StopRecorders()
        {
            m_mainThreadTimeRecorder.Stop();
            m_batchCountRecorder.Stop();
            m_triangleCountRecorder.Stop();
            m_drawCallCountRecorder.Stop();
        }

        void ResetRecorders()
        {
            m_mainThreadTimeRecorder.Reset();
            m_batchCountRecorder.Reset();
            m_triangleCountRecorder.Reset();
            m_drawCallCountRecorder.Reset();
        }
    }
}
