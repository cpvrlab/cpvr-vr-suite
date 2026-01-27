using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.Runtime.UI
{
    public class SceneHandler : MonoBehaviour, ISceneHandler
    {
        public event Action SceneChangeStarted;
        public event Action SceneChangeCompleted;

        public void ChangeScene(int index)
        {
            if (index == SceneManager.GetActiveScene().buildIndex) return;

            SceneChangeStarted?.Invoke();
            var sceneChangeOperation = SceneManager.LoadSceneAsync(index);
            sceneChangeOperation.completed += _ => SceneChangeCompleted?.Invoke();
        }
    }
}
