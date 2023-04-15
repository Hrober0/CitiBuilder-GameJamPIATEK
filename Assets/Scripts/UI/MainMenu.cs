using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad;

    [Header("Main buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _startButton.onClick.AddListener(LoadFirstLevel);
        _settingsButton.onClick.AddListener(OpenSettings);
        _creditsButton.onClick.AddListener(OpenCredits);
        _exitButton.onClick.AddListener(ExitGame);
    }

    public void LoadFirstLevel()
    {
        Debug.Log("Load game");
        SceneManager.LoadSceneAsync(_sceneToLoad);
    }

    public void OpenSettings()
    {
        Debug.Log("Open settings");
    }
    public void CloseSettings()
    {
        Debug.Log("Close settings");
    }

    public void OpenCredits()
    {
        Debug.Log("Open credits");
    }
    public void CloseCredits()
    {
        Debug.Log("Close credits");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Exit");
    }
}
