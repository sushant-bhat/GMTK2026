using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private SessionSettingsSO sessionSettings;

    void Awake()
    {
        Events.SCENE_CHANGE_EVENT.AddListener(LoadLevel);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // sessionSettings.lastLevelPlayed = GetSceneNameByIndex(currentSceneIndex + 1);
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    private void LoadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadLevel(string sceneName)
    {
        if (sceneName.Equals(""))
        {
            LoadNextLevel();
        } else if (sceneName.Equals("current"))
        {
            LoadCurrentLevel();
        } else {
            sessionSettings.lastLevelPlayed = sceneName;
            SceneManager.LoadScene(sceneName);
        }
    }

    // public async void LoadLevelAsync(string sceneName)
    // {
    //     Debug.Log("Starting background scene load...");

    //     // LoadSceneAsync in Unity 6 returns a standard Awaitable object
    //     AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

    //     // Loop to track percentage progress while loading
    //     while (!op.isDone)
    //     {
    //         float progress = Mathf.Clamp01(op.progress / 0.9f);
    //         Debug.Log($"Loading Progress: {progress * 100}%");
            
    //         // Yield control back to the engine until the next frame
    //         await Awaitable.NextFrameAsync();
    //     }

    //     Debug.Log("Scene loaded successfully!");
    // }

    public static string GetSceneNameByIndex(int buildIndex)
    {
        // Safety check to ensure the index exists in your Build Settings
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Build index {buildIndex} is out of range! Total scenes in build: {SceneManager.sceneCountInBuildSettings}");
            return string.Empty;
        }

        // Returns a path like "Assets/Scenes/MainMenu.unity"
        string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        
        // Converts the path down to just "MainMenu"
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        return sceneName;
    }
}
