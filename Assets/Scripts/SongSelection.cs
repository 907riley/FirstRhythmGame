using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using TMPro;
using UnityEngine.SceneManagement;


public class SongSelection : MonoBehaviour
{
    // prefab
    [SerializeField] GameObject songButton;
    [SerializeField] GameObject songScrollPanel;
    [SerializeField] TextMeshProUGUI errorMessage;

    // the selected song
    public string selectedSong = "";

    // error message to dispaly if start is clicked with no song selected
    private string errorMessageText = "select a song!";

    // Start is called before the first frame update
    void Start()
    {
        // ensure that the selected song is not set already
        GameManager.Instance.selectedSongName = "";
        PopulateSongSelection();
    }

    /// <summary>
    /// Get all the song names from the assets folder
    /// </summary>
    void PopulateSongSelection()
    {
        string pathToMidi = $"{Application.dataPath}/MidiPlayer/Resources/MidiDB";
        List<string> songNames = new List<string>();

        var info = new System.IO.DirectoryInfo(pathToMidi);
        var fileInfo = info.GetFiles();

        // skip the first non song
        for (int i = 0; i < fileInfo.Length; ++i)
        {
            string songName = fileInfo[i].Name.Split(".")[0];
            if (!songNames.Contains(songName))
            {
                songNames.Add(songName);

                GameObject newSong = Instantiate(songButton);
                SongButton songButtonScript = newSong.GetComponent<SongButton>();
                songButtonScript.songSelectionGo = gameObject;
                songButtonScript.songName = songName;
                newSong.GetComponentInChildren<TextMeshProUGUI>().text = songName;
                newSong.transform.parent = songScrollPanel.transform;
            }

        }
    }

    /// <summary>
    /// Called by the song selection menu to set the song name
    /// </summary>
    /// <param name="songName"> the name of the song to play </param>
    public void SetSelectedSong(string songName)
    {
        selectedSong = songName;
        errorMessage.text = "";
    }

    /// <summary>
    /// Called when the start button is clicked
    /// </summary>
    public void OnStartClick()
    {
        if (selectedSong == "")
        {
            errorMessage.text = errorMessageText;
        } else
        {
            GameManager.Instance.selectedSongName = selectedSong;
            // change scenes
            StartCoroutine(Utils.LoadAsyncScene("GamePlay"));
        }
    }
}
