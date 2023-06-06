using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    /*
     * SunnyDay is 100BPM
     */

    public float songBpm;
    public float secondsPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    // Seconds passed since song started
    public float dspSongTime = 0;
    // Offset to the first beat of the song in seconds
    public float timer = 0f;
    public AudioSource musicSource;


    // SONG SPECIFIC STUFF

    // bpm of song
    float bpm = 100f;
    // keep all note-positions-in-beats for the song
    // beat position starts at 0
    float[] notes = { 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };
    // the index of the next note to spawn
    int nextIndex = 0;
    public float beatsShownInAdvance = 3f;
    public float secondsShownInAdvance;
    public bool songPlaying = false;
    // delay of the start of playing the music in beats
    // DELAY MUST BE GREATER THAN BEATSSHOWNINADVANCE
    public int delayOfSong = 4;

    [SerializeField] GameObject noteSpawner;

    // NOTE SPECIFIC STUFF
    float spawnHeight = 5f;
    float removeHeight = -4f;


    void Start()
    {
        musicSource = GetComponent<AudioSource>();

        // calculate the number of seconds in each beat
        secondsPerBeat = 60f / songBpm;

        secondsShownInAdvance = beatsShownInAdvance * secondsPerBeat;
        //Debug.Log(secondsShownInAdvance);
        StartAudio();
    }

    void Update()
    {
        //if (!songPlaying)
        //{
        //    // wait delayOfSong beats for the song to start
        //    if (timer >= delayOfSong * secondsPerBeat)
        //    {
        //        StartAudio();
        //    }
        //    else
        //    {
        //        timer += Time.deltaTime;
        //    }
        //} 

        // determine seconds since song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        // determine beats since song started
        songPositionInBeats = songPosition / secondsPerBeat;

        if (nextIndex < notes.Length && notes[nextIndex] <= songPositionInBeats + beatsShownInAdvance)
        {
            //Instantiate(music note)
            noteSpawner.GetComponent<NoteSpawner>().SpawnNote(
                new Vector3(1, spawnHeight, 1),
                new Vector3(1, removeHeight, 1),
                beatsShownInAdvance,
                notes[nextIndex],
                transform.gameObject
                );

            // Init the fields of the music note
            nextIndex++;
        }

    }

    void StartAudio()
    {
        // Record time when music starts
        dspSongTime = (float)AudioSettings.dspTime;

        musicSource.Play();
        songPlaying = true;
        timer = 0;
    }
}
