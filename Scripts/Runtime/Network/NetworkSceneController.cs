using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        NetworkController.Instance.NetworkSceneController = this;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void LoadSceneRpc(string sceneName)
    {
        NetworkController.Instance.NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
