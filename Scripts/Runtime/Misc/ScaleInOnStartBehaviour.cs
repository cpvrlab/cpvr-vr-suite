using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Misc
{
    public class ScaleInOnStartBehaviour : MonoBehaviour
    {
        public GameObject objectToScaleIn;
        public List<GameObject> objectsToShowAfterFadeIn;
        public float transitionTime = 3.0f;

        void Start()
        {
            if (Application.isEditor) return;
            StartCoroutine(ScaleIn());
        }


        IEnumerator ScaleIn()
        {
            foreach (var objToActivate in objectsToShowAfterFadeIn)
                objToActivate.SetActive(false);

            objectToScaleIn.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);

            for (float i = 0; i < transitionTime; i += Time.deltaTime)
            {
                float scaleY = i / transitionTime;
                objectToScaleIn.transform.localScale = new Vector3(1.0f, scaleY, 1.0f);

                yield return null;
            }

            objectToScaleIn.transform.localScale = Vector3.one;

            foreach (var objToActivate in objectsToShowAfterFadeIn)
                objToActivate.SetActive(true);
        }
    }
}
