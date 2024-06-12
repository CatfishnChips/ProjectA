using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiplayerSettingsUI : MonoBehaviour
{
    public Button startServerBtn;
    public Button hostGameButton;
    public Button joinMenuButton;
    public Button joinGameButton;
    public InputField serverIpInput;

    void Start(){
        hostGameButton.onClick.AddListener(OnHostBtnClicked);
        joinGameButton.onClick.AddListener(OnConnecBtnClicked);
        startServerBtn.onClick.AddListener(OnServerBtnClicked);
        Debug.Log("Added listeners");
    }

    private void OnHostBtnClicked(){
        Debug.Log("Trying to load the scene.");
        MultiplayerConnectionData.networkRole = MultiplayerConnectionData.NetworkRole.Host;
        SceneManager.LoadScene("TrainingScene Online");
    }

    private void OnConnecBtnClicked(){
        if(!string.IsNullOrEmpty(serverIpInput.text)){
            MultiplayerConnectionData.ServerIP = serverIpInput.text;
            MultiplayerConnectionData.networkRole = MultiplayerConnectionData.NetworkRole.Client;
            SceneManager.LoadScene("TrainingScene Online");
        }
        else{
            Debug.LogError("Server IP is Empty");
        }
    }

    private void OnServerBtnClicked(){
        MultiplayerConnectionData.networkRole = MultiplayerConnectionData.NetworkRole.Server;
        SceneManager.LoadScene("TrainingScene Online");
    }

}
