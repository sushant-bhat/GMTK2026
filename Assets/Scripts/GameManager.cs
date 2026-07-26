using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string startLevel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject levelOverMenu;
    [SerializeField] private SessionSettingsSO sessionSettings;

    void Awake()
    {
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
        Events.GAME_PAUSE_EVENT.AddListener(PauseGame);
        Events.LEVEL_OVER_EVENT.AddListener(OnLevelOver);
    }

    private void OnLevelOver(LevelOverReason reason)
    {
        Time.timeScale = 0;
    }

    void Start()
    {
        Time.timeScale = 1;
        Events.SCENE_CHANGE_EVENT.Invoke(startLevel);
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
    }

    public void StartNewGame()
    {
        Time.timeScale = 1;
        Events.SCENE_CHANGE_EVENT.Invoke("TutorialScene");
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        Events.SCENE_CHANGE_EVENT.Invoke("");
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
    }

    public void ReloadLevel()
    {
        Events.SCENE_CHANGE_EVENT.Invoke("current");
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
        Events.GAME_RESUME_EVENT.Invoke();
    }

    public void ResumeLastPlayed()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
        Events.SCENE_CHANGE_EVENT.Invoke(sessionSettings.lastLevelPlayed);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
        Events.SCENE_CHANGE_EVENT.Invoke("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
