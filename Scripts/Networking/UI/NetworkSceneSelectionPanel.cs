using Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    /// <summary>
    /// Ask for a network scene change.
    /// </summary>
    public class NetworkSceneSelectionPanel : SceneSelectionPanel {
        public override void ChangeScene(int index) {
            if (NetworkManager.Singleton.SceneManager == null) {
                base.ChangeScene(index);
                return;
            }

            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(index));

            if (MyNetworkSceneManager.Instance.LoadScene(sceneName)) {
                RemoveDynamicPanels();
            }
        }
    }
}
