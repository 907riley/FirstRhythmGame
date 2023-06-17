using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class Settings : MonoBehaviour
{
    // temp settings so that we can have a save button
    private int numberOfFingerButtons;
    private float beatsShownInAdvance;

    // UI items
    [SerializeField] TMP_Dropdown fingerButtonsDropdown;
    [SerializeField] Slider beatsShownInAdvanceSlider;

    private void Start()
    {
        // init vars from GameManager to fill the settings
        numberOfFingerButtons = GameManager.Instance.numberOfFingerButtons;
        beatsShownInAdvance = GameManager.Instance.beatsShownInAdvance;

        // set the UI elements to match the defaults
        fingerButtonsDropdown.value = numberOfFingerButtons;
        beatsShownInAdvanceSlider.value = beatsShownInAdvance;
    }

    /// <summary>
    /// Called when the slider changes for beatsShownInAdvance
    /// </summary>
    public void OnSpeedChange()
    {
        beatsShownInAdvance =  beatsShownInAdvanceSlider.value;
    }

    /// <summary>
    /// Called when the dropdown selects a new number for numberOfFingerButtons
    /// </summary>
    public void OnNumberOfFingerButtonsChange()
    {
        // if fails, just revert to what it was
        int.TryParse(fingerButtonsDropdown.options[fingerButtonsDropdown.value].text, out numberOfFingerButtons);
    }

    /// <summary>
    /// Called when the back button is clicked
    /// </summary>
    public void OnBackClick()
    {
        StartCoroutine(LoadAsyncScene("Menu"));
    }

    public void OnSaveClick()
    {
        // update the GameManager
        GameManager.Instance.SetNumberOfFingerButtons(numberOfFingerButtons);
        GameManager.Instance.SetBeatsShownInAdvance(beatsShownInAdvance);
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
