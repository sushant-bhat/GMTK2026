using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    void Awake()
    {
        Events.SCENE_CHANGE_EVENT.AddListener(LoadLevel);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadLevel(string sceneName)
    {
        if (sceneName.Equals(""))
        {
            LoadNextLevel();
        } else {
            SceneManager.LoadScene(sceneName);
        }
    }

    public async void LoadLevelAsync(string sceneName)
    {
        Debug.Log("Starting background scene load...");

        // LoadSceneAsync in Unity 6 returns a standard Awaitable object
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // Loop to track percentage progress while loading
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log($"Loading Progress: {progress * 100}%");
            
            // Yield control back to the engine until the next frame
            await Awaitable.NextFrameAsync();
        }

        Debug.Log("Scene loaded successfully!");
    }
}
