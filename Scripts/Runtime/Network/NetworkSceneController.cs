using Unity.Netcode;
using UnityEngine.SceneManagement;
using Util;

public class NetworkSceneController : NetworkSingleton<NetworkSceneController>
{
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void LoadSceneRpc(string sceneName)
    {
        NetworkController.Instance.NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
