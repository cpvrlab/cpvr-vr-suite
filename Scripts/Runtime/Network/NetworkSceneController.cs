using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        NetworkController.Instance.NetworkSceneController = this;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void LoadSceneRpc(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
