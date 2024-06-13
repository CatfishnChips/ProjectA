using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;

public class NetworkConnectionHandler : MonoBehaviour
{

    [SerializeField] private Text hostIpText;
    [SerializeField] private Text hostPortText;

    // Start is called before the first frame update
    void Start()
    {
        if(MultiplayerConnectionData.networkRole == MultiplayerConnectionData.NetworkRole.Client){
            StartClient();
        }
        else if(MultiplayerConnectionData.networkRole == MultiplayerConnectionData.NetworkRole.Host){
            StartHost();
        }
        else if(MultiplayerConnectionData.networkRole == MultiplayerConnectionData.NetworkRole.Server){
            StartServer();
        }

        //StartHost();

    }

    void StartHost(){
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = NetworkUtils.GetLocalIPAddress();
        NetworkManager.Singleton.StartHost();
        hostIpText.text = NetworkUtils.GetLocalIPAddress();
        hostPortText.text = NetworkUtils.GetPort().ToString();
    }

    void StartClient(){
        try
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = MultiplayerConnectionData.ServerIP;
            NetworkManager.Singleton.StartClient();
        }
        catch (NetworkConfigurationException e)
        {
            
            Debug.LogError(e);
            throw;
        }
    }

    void StartServer(){
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = NetworkUtils.GetLocalIPAddress();
        NetworkManager.Singleton.StartServer();
        hostIpText.text = NetworkUtils.GetLocalIPAddress();
        hostPortText.text = NetworkUtils.GetPort().ToString();
        FindObjectOfType<ServerManager>().SetupServer();
    }

    public void Shutdown(){
        NetworkManager.Singleton.Shutdown();
    }

}
