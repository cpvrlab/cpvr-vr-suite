using System;
using System.Collections;
using System.Threading.Tasks;
using cpvrlab_vr_suite.Scripts.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.UI
{
    public class ScreenshotMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Image flashImage;
        [SerializeField] private Button screenshotButton;

        [Header("Screenshot Settings")]
        [SerializeField] private string emailAddress;
        [SerializeField] private bool saveImageToGallery;

        private void Start()
        {
            countdownText.text = "";
            resultText.text = "";
            flashImage.color = new Color(0, 0, 0, 0);

            if (!emailAddress.Equals(""))
                PlayerPrefs.SetString("emailAddress", emailAddress);
            else
                emailAddress = PlayerPrefs.GetString("emailAddress");
            if (emailAddress.Equals("") && !saveImageToGallery)
                screenshotButton.interactable = false;
        }

        public void OnScreenshotClicked() => ScreenshotRoutine();

        private async void ScreenshotRoutine()
        {
            var flashColor = Color.white;
            var filename = $"VR4Architects-{DateTime.Now:yyyyMMdd-HHmmss}";

            screenshotButton.interactable = false;
        
            // Countdown
            for (var i = 4; i > 0; i--)
            {
                countdownText.text = i.ToString();
                await Task.Delay(1000);
            }
            countdownText.text = "";

            await Task.Yield();
        
            // Take Screenshot
            var screenshot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.LeftEye);

            await Task.Yield();

            // Flash Screen
            flashImage.color = flashColor;
            for (float i = 0; i < 0.33f; i += Time.deltaTime)
            {
                flashColor.a = Mathf.Lerp(1, 0, i / 0.33f);
                flashImage.color = flashColor;
                await Task.Yield();
            }
        
            screenshotButton.interactable = true;
            flashImage.color = new Color(0, 0, 0, 0);
        
            // Send Mail
            var resultMsg = "";

            if (!emailAddress.Equals(""))
            {
                var emailError = await MailSender.SendEmail(emailAddress, "", screenshot);
                if (emailError)
                    resultMsg = "Screenshot sent to " + emailAddress;
                else
                    resultMsg = "Error sending screenshot to " + emailAddress;
            }
        
            // Save to local Storage (Quest)
#if UNITY_ANDROID
            if (saveImageToGallery)
            {
                var imageURL = AndroidExtensions.SaveImageToGallery(screenshot, filename, "");
                resultMsg += "\n Screenshot saved to: \n" + imageURL;
            }
#endif

            // Show result message
            if (!resultMsg.Equals(""))
            {
                resultText.text = resultMsg.ToString();

                for (var i = 3; i > 0; i--)
                {
                    await Task.Delay(1000);
                }
            
                Debug.Log(resultMsg);
            
                resultText.text = "";
                countdownText.text = "";
            }
        }
    }
}
