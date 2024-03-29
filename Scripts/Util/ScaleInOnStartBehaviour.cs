using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Util
{
    public class ScaleInOnStartBehaviour : MonoBehaviour
    {
        public GameObject objectToScaleIn;
        public List<GameObject> objectsToShowAfterFadeIn;
        public float transitionTime = 3.0f;
        
        private InitializeTeleportationAreas _initTeleportAreaScript; 

        private void Awake()
        {
            objectToScaleIn.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);

            foreach (var objToActivate in objectsToShowAfterFadeIn)
                objToActivate.SetActive(false);

            _initTeleportAreaScript = FindObjectOfType<InitializeTeleportationAreas>();
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(ScaleIn());
        }


        private IEnumerator ScaleIn()
        {
            for (float i = 0; i < transitionTime; i += Time.deltaTime)
            {
                float scaleY = i / transitionTime;
                objectToScaleIn.transform.localScale = new Vector3(1.0f, scaleY, 1.0f);

                Debug.Log("ScaleInFactor: " + scaleY);
                yield return null;
            }

            foreach (var objToActivate in objectsToShowAfterFadeIn)
                objToActivate.SetActive(true);

            if (_initTeleportAreaScript != null)
                _initTeleportAreaScript.CreateTeleportAreas(SceneManager.GetActiveScene());
        }
    }
}
