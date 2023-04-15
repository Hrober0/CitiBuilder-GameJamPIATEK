using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


    public void LoadFirstLevel()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void OpenSettings()
    {

    }
    
    public void CloseSettings()
    {

    }

    public void OpenCredits()
    {

    }

    public void CloseCredits()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Exit");
    }
}
