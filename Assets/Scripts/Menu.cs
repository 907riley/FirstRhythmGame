using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    /// <summary>
    /// Called when the play button is clicked
    /// </summary>
    public void OnPlayClick()
    {
        // Load the GamePlay scene
        StartCoroutine(Utils.LoadAsyncScene("SongSelection"));
    }

    /// <summary>
    /// Called when the settings button is clicked
    /// </summary>
    public void OnSettingsClick()
    {
        // Load the Settings scene
        StartCoroutine(Utils.LoadAsyncScene("Settings"));
    }

    /// <summary>
    /// Called when the Quit button is clicked
    /// </summary>
    public void OnQuitClick()
    {
        Application.Quit();
    }
}
