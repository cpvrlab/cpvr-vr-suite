using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using cpvrlab_vr_suite.Scripts.Util;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class InfoController : Controller
{
    readonly TMP_Text m_countdownText;
    readonly TMP_Text m_resultText;
    readonly Image m_flashImage;
    readonly TMP_Text m_fpsText;
    readonly TMP_Text m_debugText;
    float m_deltaTime;
    ProfilerRecorder m_drawBatchCountRecorder;
    ProfilerRecorder m_drawCallCountRecorder;
    ProfilerRecorder m_triangleCountRecorder;
    readonly Dictionary<string, string> m_debugLogs = new();

    public InfoController(View view,
                        CanvasManager canvasManager,
                        TMP_Text countdownText,
                        TMP_Text resultText,
                        Image flashImage,
                        TMP_Text fpsText,
                        TMP_Text debugText)
        : base(view, canvasManager)
    {
        m_countdownText = countdownText;
        m_resultText = resultText;
        m_flashImage = flashImage;
        m_fpsText = fpsText;
        m_debugText = debugText;
        m_countdownText.text = string.Empty;
        m_resultText.text = string.Empty;
        m_flashImage.color = Color.clear;
        m_fpsText.text = string.Empty;
        m_debugText.text = string.Empty;

        SetFPSTextState(PlayerPrefs.GetInt("showFPS") == 1);
        SetDebugLogTextState(PlayerPrefs.GetInt("showDebug") == 1);
    }

    public void SetFPSTextState(bool state)
    {
        m_fpsText.enabled = state;

        if (state)
        {
            m_drawBatchCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Total Batches Count");
            m_drawCallCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            m_triangleCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }
        else
        {
            m_drawBatchCountRecorder.Dispose();
            m_drawCallCountRecorder.Dispose();
        }
    }

    public void UpdateFPSText()
    {
        if (!m_fpsText.enabled) return;

        var sb = new StringBuilder();

        m_deltaTime += (Time.unscaledDeltaTime - m_deltaTime) * 0.01f;
        var currentFPS = (1.0f / m_deltaTime).ToString("0.0");

        sb.AppendLine($"Draw Calls: {m_drawCallCountRecorder.LastValue}");
        sb.AppendLine($"Batches: {m_drawBatchCountRecorder.LastValue}");
        sb.AppendLine($"Triangles: {m_triangleCountRecorder.LastValue}");
        sb.AppendLine($"FPS: {currentFPS}");

        m_fpsText.text = sb.ToString();
    }

    public void SetDebugLogTextState(bool state)
    {
        m_debugText.enabled = state;

        if (state)
            Application.logMessageReceived += HandleLog;
        else
            Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log ||
        type == LogType.Exception ||
        type == LogType.Warning ||
        type == LogType.Error)
        {
            var splitString = logString.Split(char.Parse(":"));
            var debugKey = splitString[0];
            var debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (m_debugLogs.ContainsKey(debugKey))
                m_debugLogs[debugKey] = debugValue;
            else
                m_debugLogs.Add(debugKey, debugValue);
        }

        var displayText = "";
        foreach (KeyValuePair<string, string> log in m_debugLogs)
        {
            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += log.Key + ": " + log.Value + "\n";
        }

        m_debugText.text = displayText;
    }
    
    public async Task TakeScreenshot(string email, bool saveToGallery = false)
    {
        var flashColor = Color.white;
        var filename = $"VR4Architects-{DateTime.Now:yyyyMMdd-HHmmss}";

        // Countdown
        for (var i = 4; i > 0; i--)
        {
            m_countdownText.text = i.ToString();
            await Task.Delay(1000);
        }
        m_countdownText.text = "";

        await Task.Yield();

        // Take Screenshot
        var screenshot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.LeftEye);

        await Task.Yield();

        // Flash Screen
        m_flashImage.color = flashColor;
        await Task.Yield();

        for (float i = 0; i < 0.33f; i += Time.deltaTime)
        {
            flashColor.a = Mathf.Lerp(1, 0, i / 0.33f);
            m_flashImage.color = flashColor;
            await Task.Yield();
        }

        m_flashImage.color = new Color(0, 0, 0, 0);

        // Send Mail
        var resultMsg = "";

        if (MailSender.IsValidEmail(email))
        {
            var emailSent = await MailSender.SendEmail(email, "", screenshot);
            if (emailSent)
                resultMsg = "Screenshot sent to " + email;
            else
                resultMsg = "Error sending screenshot to " + email;
        }

        // Save to local Storage (Quest)
#if UNITY_ANDROID
        if (saveToGallery)
        {
            var imageURL = AndroidExtensions.SaveImageToGallery(screenshot, filename, "");
            resultMsg += "\n Screenshot saved to: \n" + imageURL;
        }
#endif

        // Show result message
        if (!resultMsg.Equals(""))
        {
            m_resultText.text = resultMsg.ToString();

            for (var i = 3; i > 0; i--)
            {
                await Task.Delay(1000);
            }

            Debug.Log(resultMsg);

            m_resultText.text = "";
            m_countdownText.text = "";
        }
    }
}
