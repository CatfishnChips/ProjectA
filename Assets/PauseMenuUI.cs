using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{

    public GameObject MovesetLayout;

    public void OnMainMenuButton(){
        SceneManager.LoadScene("MainMenu");
    }

    public void OnMoveSetButton(){
        
    }
}
