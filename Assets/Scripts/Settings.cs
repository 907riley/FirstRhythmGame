using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class Settings : MonoBehaviour
{
    private int numberOfFingerButtons;
    private float beatsShownInAdvance;

    [SerializeField] TMP_Dropdown fingerButtonsDropdown;
    [SerializeField] Slider beatsShownInAdvanceSlider;

    private void Start()
    {
        // init vars from GameManager to fill the settings
        numberOfFingerButtons = GameManager.Instance.numberOfFingerButtons;
        beatsShownInAdvance = GameManager.Instance.beatsShownInAdvance;

        fingerButtonsDropdown.value = numberOfFingerButtons;
        beatsShownInAdvanceSlider.value = beatsShownInAdvance;
    }

    public void OnSpeedChange()
    {
        beatsShownInAdvance =  beatsShownInAdvanceSlider.value;
    }

    public void OnNumberOfFingerButtonsChange()
    {
        // if fails, just revert to what it was
        int.TryParse(fingerButtonsDropdown.options[fingerButtonsDropdown.value].text, out numberOfFingerButtons);
    }

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
