using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayClick()
    {
        // Load the GamePlay scene
        StartCoroutine(LoadAsyncScene("GamePlay"));
    }

    public void OnSettingsClick()
    {
        // Load the Settings scene
        StartCoroutine(LoadAsyncScene("Settings"));
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    IEnumerator LoadAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // wait till async is done
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
