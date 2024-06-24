using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    /// <summary>
    /// The behaviour of the markers.
    /// </summary>
    public class MarkerBehaviour : MonoBehaviour
    {
        // A reference to the number of the marker.
        [SerializeField] private TMP_Text numberText;
        // The image (circle)
        [SerializeField] private Image targetImage;

        // Colors of the components.
        public Color numberColor;
        public Color ringColor;

        // If the object is just the reticle of the ray.
        private bool _isReticle;

        public bool IsReticle
        {
            get => _isReticle;
            set
            {
                // Set the reticle color as transparent if reticle
                _isReticle = value;
                float alpha = _isReticle ? .3f : 1f;

                ringColor[3] = alpha;
                numberColor[3] = alpha;

                numberText.color = numberColor;
                targetImage.color = ringColor;
            }
        }

        private void Awake()
        {
            targetImage.color = ringColor;
            numberText.color = numberColor;
        }

        private void FixedUpdate()
        {
            // Look at camera
            Vector3 cameraPosition = GameObject.FindWithTag("MainCamera").transform.position;
            cameraPosition.Scale(new Vector3(1, 0, 1));

            Vector3 toCamera = cameraPosition - transform.position;

            float yAngle = -Vector3.SignedAngle(toCamera, Vector3.back, Vector3.up);

            transform.rotation = Quaternion.Euler(0, yAngle, 0);
        }

        public void SetNumber(int number)
        {
            numberText.text = number.ToString();
        }
    }
}
