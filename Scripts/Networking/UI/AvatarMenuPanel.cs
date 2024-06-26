using TMPro;
using UnityEngine.UI;
using V3.Scripts.VR;

namespace UI
{
    /// <summary>
    /// Used to recalibrate the height of the avatar if needed.
    /// </summary>
    public class AvatarMenuPanel : MenuPanel
    {
        public AvatarBehaviour avatarBehaviour;

        public Button calibrateHeightButton;
        public TMP_Text heightLabel;

        /// <summary>
        /// Recalibrate height.
        /// </summary>
        public void CalibrateHeight()
        {
            if (avatarBehaviour.CalibrateHeight())
            {
                heightLabel.text = "Calibrating...";
                calibrateHeightButton.interactable = false;
            }
        }

        /// <summary>
        /// Display the calibrated height.
        /// </summary>
        /// <param name="height">New height.</param>
        public void CalibrationFinished(float height)
        {
            calibrateHeightButton.interactable = true;
            heightLabel.text = "Height: " + (height + .05).ToString("0.00") + "m";
        }
    }
}
