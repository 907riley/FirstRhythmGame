using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongButton : MonoBehaviour
{
    public GameObject songSelectionGo;
    public string songName;

    public void OnSongClick()
    {
        songSelectionGo.GetComponent<SongSelection>().SetSelectedSong(songName);
    }
}
