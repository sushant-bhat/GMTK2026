using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string startLevel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject levelOverMenu;

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
        if (reason.Equals(LevelOverReason.ENEMIES_KILLED))
        {
            levelOverMenu.SetActive(true);
        } else
        {
            levelOverMenu.SetActive(true);
        }
    }

    void Start()
    {
        Events.SCENE_CHANGE_EVENT.Invoke(startLevel);
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
    }

    public void StartNewGame()
    {
        Events.SCENE_CHANGE_EVENT.Invoke("TutorialScene");
        pauseMenu.SetActive(false);
        levelOverMenu.SetActive(false);
    }

    public void LoadNextLevel()
    {
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
