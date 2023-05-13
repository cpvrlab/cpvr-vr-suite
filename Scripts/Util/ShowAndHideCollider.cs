using System.Collections.Generic;
using UnityEngine;

namespace cpvrlab_vr_suite.Scripts.Util
{
    /// <summary>
    /// This is a generic behaviour to show and hide objects when the main camera tagged game object
    /// is entering and exiting the collider.
    /// </summary>
    public class ShowAndHideCollider : MonoBehaviour
    {
        public List<GameObject> objectsToShow;
        public List<GameObject> objectsToHide;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("MainCamera")) return;

            Debug.Log("OnTriggerEnter: " + gameObject.name);

            foreach (var toShow in objectsToShow)
                toShow.SetActive(true);
            foreach (var toHide in objectsToHide)
                toHide.SetActive(false);

        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("MainCamera")) return;

            Debug.Log("OnTriggerExit: " + gameObject.name);

            foreach (var toShow in objectsToShow)
                toShow.SetActive(false);
            foreach (var toHide in objectsToHide)
                toHide.SetActive(true);
        }
    }
}
