using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class GazeManager : InteractorManager
    {
        public bool Operative { get; private set; }
        public bool Blocked { get; private set; }

        void Awake() => Initialize();

        void Initialize()
        {
            Operative = !PlayerPrefs.HasKey("useGaze") || PlayerPrefs.GetInt("useGaze") == 1;
            Blocked = false;
            gameObject.SetActive(Operative && !Blocked);
        }

        public void SetActiveState(bool value)
        {
            Operative = value;
            gameObject.SetActive(Operative && !Blocked);
            PlayerPrefs.SetInt("useGaze", Operative ? 1 : 0);
        }

        public void BlockInteractor(bool value)
        {
            Blocked = value;
            gameObject.SetActive(Operative && !Blocked);
        }
    }
}
