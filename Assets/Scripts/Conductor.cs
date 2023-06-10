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
    public float offSet = 0f;
    public AudioSource musicSource;
    // SONG SPECIFIC STUFF

    // bpm of song
    //float bpm = 100f;
    // keep all note-positions-in-beats for the song
    // beat position starts at 0
    public struct NoteInformation
    {
        public NoteInformation(float beat, int noteIndex)
        {
            this.beat = beat;
            this.noteIndex = noteIndex;
        }

        public float beat;
        public int noteIndex;
    }

    NoteInformation[] notes;
    //float[] notes;
    // the index of the next note to spawn
    int nextIndex = 0;
    public float beatsShownInAdvance = 2f;
    public float secondsShownInAdvance;
    public bool songPlaying = false;
    // delay of the start of playing the music in beats
    // DELAY MUST BE GREATER THAN BEATSSHOWNINADVANCE
    public int delayOfSong = 5;

    [SerializeField] GameObject noteSpawner;

    [SerializeField] GameObject gameManagerGo;
    private GameManager gameManager;

    // NOTE SPECIFIC STUFF
    float spawnHeight;
    float fingerBoardHeight;
    float removeHeight;
    public float noteFallLerpPercent;
    private string[] noteNames;


    void Start()
    {
        //gameManager = gameManagerGo.GetComponent<GameManager>();
        //spawnHeight = gameManager.spawnHeight;
        //fingerBoardHeight = gameManager.fingerBoardHeight;
        //removeHeight = gameManager.removeHeight;
        //noteNames = gameManager.noteNames;

        spawnHeight = GameManager.Instance.spawnHeight;
        fingerBoardHeight = GameManager.Instance.fingerBoardHeight;
        removeHeight = GameManager.Instance.removeHeight;
        noteNames = GameManager.Instance.noteNames;

        SetupBeatsToPlay();

        // important for knowing when the note needs to pass the fingerbutton
        // since we want to continue LERPing the note pass the fingerboard
        noteFallLerpPercent = (spawnHeight - fingerBoardHeight) / (spawnHeight - removeHeight);
        //Debug.Log((spawnHeight - fingerBoardHeight) + " " + (spawnHeight - removeHeight));
        //Debug.Log((spawnHeight - fingerBoardHeight) / (spawnHeight - removeHeight));
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

        if (nextIndex < notes.Length && notes[nextIndex].beat <= songPositionInBeats + beatsShownInAdvance)
        {
            //Instantiate(music note)
            noteSpawner.GetComponent<NoteSpawner>().SpawnNote(
                new Vector3(1, spawnHeight, 1),
                new Vector3(1, removeHeight, 1),
                beatsShownInAdvance,
                notes[nextIndex].beat,
                notes[nextIndex].noteIndex
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
        //timer = 0;
    }

    // don't have to clutter the rest of the file
    void SetupBeatsToPlay()
    {
        notes = new NoteInformation[49];

        int noteCounter = 0;

        // 4 GREENS
        notes[noteCounter++] = new NoteInformation(5f, 0);
        notes[noteCounter++] = new NoteInformation(6f, 0);
        notes[noteCounter++] = new NoteInformation(7f, 0);
        notes[noteCounter++] = new NoteInformation(8f, 0);

        // 4 REDS
        notes[noteCounter++] = new NoteInformation(9f, 1);
        notes[noteCounter++] = new NoteInformation(10f, 1);
        notes[noteCounter++] = new NoteInformation(11f, 1);
        notes[noteCounter++] = new NoteInformation(12f, 1);

        // 4 GREENS
        notes[noteCounter++] = new NoteInformation(13f, 0);
        notes[noteCounter++] = new NoteInformation(14f, 0);
        notes[noteCounter++] = new NoteInformation(15f, 0);
        notes[noteCounter++] = new NoteInformation(16f, 0);

        // 4 REDS
        notes[noteCounter++] = new NoteInformation(17f, 1);
        notes[noteCounter++] = new NoteInformation(18f, 1);
        notes[noteCounter++] = new NoteInformation(19f, 1);
        notes[noteCounter++] = new NoteInformation(20f, 1); // 16

        // alternating GREENS and YELLOWS
        notes[noteCounter++] = new NoteInformation(21f, 0);
        notes[noteCounter++] = new NoteInformation(21.5f, 2);
        notes[noteCounter++] = new NoteInformation(22f, 0);
        notes[noteCounter++] = new NoteInformation(22.5f, 2);

        notes[noteCounter++] = new NoteInformation(23f, 0);
        notes[noteCounter++] = new NoteInformation(23.5f, 2);
        notes[noteCounter++] = new NoteInformation(24f, 0);
        notes[noteCounter++] = new NoteInformation(24.5f, 2);

        notes[noteCounter++] = new NoteInformation(25f, 0);

        // alternating REDS and BLUES
        notes[noteCounter++] = new NoteInformation(25.5f, 3);
        notes[noteCounter++] = new NoteInformation(26f, 1);
        notes[noteCounter++] = new NoteInformation(26.5f, 3);

        notes[noteCounter++] = new NoteInformation(27f, 1);
        notes[noteCounter++] = new NoteInformation(27.5f, 3);
        notes[noteCounter++] = new NoteInformation(28f, 1);
        notes[noteCounter++] = new NoteInformation(28.5f, 3); // 32

        notes[noteCounter++] = new NoteInformation(29f, 1);
        notes[noteCounter++] = new NoteInformation(29.5f, 3);

        // alternating GREENS and YELLOWS
        notes[noteCounter++] = new NoteInformation(30f, 0);
        notes[noteCounter++] = new NoteInformation(30.5f, 2);

        notes[noteCounter++] = new NoteInformation(31f, 0);
        notes[noteCounter++] = new NoteInformation(31.5f, 2);
        notes[noteCounter++] = new NoteInformation(32f, 0);
        notes[noteCounter++] = new NoteInformation(32.5f, 2);

        notes[noteCounter++] = new NoteInformation(33f, 0);

        // alternating REDS and BLUES
        notes[noteCounter++] = new NoteInformation(33.5f, 3);
        notes[noteCounter++] = new NoteInformation(34f, 1);
        notes[noteCounter++] = new NoteInformation(34.5f, 3);

        notes[noteCounter++] = new NoteInformation(35f, 1);
        notes[noteCounter++] = new NoteInformation(35.5f, 3);
        notes[noteCounter++] = new NoteInformation(36f, 1);
        notes[noteCounter++] = new NoteInformation(36.5f, 3); // 48

        notes[noteCounter++] = new NoteInformation(37f, 1);
    }
}
