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
    public float firstBeatOffset = 0f;
    public AudioSource musicSource;


    // SONG SPECIFIC STUFF

    // bpm of song
    float bpm = 100f;
    // keep all note-positions-in-beats for the song
    float[] notes = { 1f/*, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f*/ };
    // the index of the next note to spawn
    int nextIndex = 0;
    public float beatsShownInAdvance = 3f;
    public float secondsShownInAdvance;
    public bool songPlaying = false;

    [SerializeField] GameObject noteSpawner;



    void Start()
    {
        musicSource = GetComponent<AudioSource>();

        // calculate the number of seconds in each beat
        secondsPerBeat = 60f / songBpm;

        secondsShownInAdvance = beatsShownInAdvance * secondsPerBeat;
        //Debug.Log(secondsShownInAdvance);
    }

    void Update()
    {
        if (!songPlaying)
        {
            if (firstBeatOffset >= secondsShownInAdvance)
            {
                StartAudio();
            }
            else
            {
                firstBeatOffset += Time.deltaTime;
            }
        } 


        // determine seconds since song started
        songPosition =  (float)(AudioSettings.dspTime - dspSongTime);

        // determine beats since song started
        songPositionInBeats = songPosition / secondsPerBeat;

        if (nextIndex < notes.Length && notes[nextIndex] < songPositionInBeats + beatsShownInAdvance)
        {
            //Instantiate(music note)
            noteSpawner.GetComponent<NoteSpawner>().SpawnNote(
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 1),
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
    }
}
