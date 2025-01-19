using UnityEngine;

namespace Util
{
    public static class MarkerPrefs
    {
        public static void SavePositon(Vector3 pos)
        {
            PlayerPrefs.SetFloat("posX", pos.x);
            PlayerPrefs.SetFloat("posY", pos.y);
            PlayerPrefs.SetFloat("posZ", pos.z);
            PlayerPrefs.Save();
        }

        public static bool LoadPosition(out Vector3 pos)
        {
            pos = Vector3.zero;

            if (!(PlayerPrefs.HasKey("posX") && PlayerPrefs.HasKey("posY") &&
                  PlayerPrefs.HasKey("posZ"))) return false;

            pos = new Vector3(
                PlayerPrefs.GetFloat("posX"),
                PlayerPrefs.GetFloat("posY"),
                PlayerPrefs.GetFloat("posZ")
            );

            return true;
        }
    }
}