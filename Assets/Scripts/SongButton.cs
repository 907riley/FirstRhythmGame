using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongButton : MonoBehaviour
{
    public GameObject songSelectionGo;

    // the name of the associated song
    public string songName;

    /// <summary>
    /// When a song is clicked set the selected song variable in the song selection gameobject
    /// </summary>
    public void OnSongClick()
    {
        songSelectionGo.GetComponent<SongSelection>().SetSelectedSong(songName);
    }
}
