using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using TMPro;
using UnityEngine.SceneManagement;


public class SongSelection : MonoBehaviour
{
    MidiFilePlayer mfp;
    List<string> songNames;
    [SerializeField] GameObject songButton;
    [SerializeField] GameObject songScrollPanel;
    [SerializeField] TextMeshProUGUI errorMessage;

    public string selectedSong = "";
    private string errorMessageText = "select a song!";

    // Start is called before the first frame update
    void Start()
    {
        // ensure that the selected song is not set already
        GameManager.Instance.selectedSongName = "";
        PopulateSongSelection();
    }

    void PopulateSongSelection()
    {
        string pathToMidi = $"{Application.dataPath}/MidiPlayer/Resources/MidiDB";
        songNames = new List<string>();

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

    public void SetSelectedSong(string songName)
    {
        selectedSong = songName;
        errorMessage.text = "";
    }

    public void OnStartClick()
    {
        if (selectedSong == "")
        {
            errorMessage.text = errorMessageText;
        } else
        {
            GameManager.Instance.selectedSongName = selectedSong;
            // change scenes
            StartCoroutine(LoadAsyncScene("GamePlay"));
        }
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
