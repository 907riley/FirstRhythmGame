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
        SceneManager.LoadScene(1);
    }

    public void OnSettingsClick()
    {
        // Load the Settings scene
        // TODO: make settings scene
        //SceneManager.LoadScene(2);
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}
