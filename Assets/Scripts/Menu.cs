using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
