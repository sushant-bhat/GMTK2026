using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string startLevel;
    [SerializeField] private GameObject pauseMenu;

    void Awake()
    {
        pauseMenu.SetActive(false);
        Events.GAME_PAUSE_EVENT.AddListener(PauseGame);
    }

    void Start()
    {
        Events.SCENE_CHANGE_EVENT.Invoke(startLevel);
    }

    public void StartNewGame()
    {
        Events.SCENE_CHANGE_EVENT.Invoke("TutorialScene");
    }

    public void LoadNextLevel()
    {
        Events.SCENE_CHANGE_EVENT.Invoke("");
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
        Events.GAME_RESUME_EVENT.Invoke();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Events.SCENE_CHANGE_EVENT.Invoke("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
