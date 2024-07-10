using UnityEngine;

namespace Util {
    /// <summary>
    /// Used to save the marker position between sessions.
    /// </summary>
    public static class MarkerPrefs {
        /// <summary>
        /// Save the markers.
        /// </summary>
        /// <param name="first">First point position.</param>
        /// <param name="second">Second point position.</param>
        public static void SavePrefs(Vector3 first, Vector3 second) {
            PlayerPrefs.SetFloat("FirstX", first.x);
            PlayerPrefs.SetFloat("FirstY", first.y);
            PlayerPrefs.SetFloat("FirstZ", first.z);
            PlayerPrefs.SetFloat("SecondX", second.x);
            PlayerPrefs.SetFloat("SecondY", second.y);
            PlayerPrefs.SetFloat("SecondZ", second.z);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load the markers.
        /// </summary>
        /// <param name="first">Out variable to get the first point position.</param>
        /// <param name="second">Out variable to get the second point position.</param>
        /// <returns>A bool that show if the markers has been found.</returns>
        public static bool LoadPrefs(out Vector3 first, out Vector3 second) {
            first = Vector3.zero;
            second = Vector3.zero;
            
            if (!(PlayerPrefs.HasKey("FirstX") && PlayerPrefs.HasKey("FirstY") &&
                  PlayerPrefs.HasKey("FirstZ") && PlayerPrefs.HasKey("SecondX") &&
                  PlayerPrefs.HasKey("SecondY") && PlayerPrefs.HasKey("SecondZ"))) return false;

            first = new Vector3(
                PlayerPrefs.GetFloat("FirstX"),
                PlayerPrefs.GetFloat("FirstY"),
                PlayerPrefs.GetFloat("FirstZ")
            );

            second = new Vector3(
                PlayerPrefs.GetFloat("SecondX"),
                PlayerPrefs.GetFloat("SecondY"),
                PlayerPrefs.GetFloat("SecondZ")
            );
            
            return true;
        }
    }
}