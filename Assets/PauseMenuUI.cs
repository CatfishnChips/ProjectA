using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{

    public NetworkConnectionHandler connectionHandler;

    public void OnMainMenuButton(){
        if(SceneManager.GetActiveScene().name == "TrainingScene Online"){
            connectionHandler.Shutdown();
        }
        SceneManager.LoadScene("MainMenu");
    }
}
