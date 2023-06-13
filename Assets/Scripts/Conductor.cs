using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class Conductor : MonoBehaviour
{
    /*
     * SunnyDay is 100BPM
     */

    public double songBpm;
    public double secondsPerBeat;
    public double songPosition;
    public float songPositionInBeats;
    public long songPositionInTicks;
    public double initDspSongTime = 0;
    // Seconds passed since song started
    public double dspSongTime = 0;
    // Offset to the first beat of the song in seconds (like for metadata if MP3)
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
    public float beatsShownInAdvance;
    public double secondsShownInAdvance;
    public bool songPlaying = false;
    public double timeSignatureNumerator;
    public double timeSignatureDenominator;
    public double notesPerSecond;
    public double secondsPerMeasure;
    // count of measures played
    public int beatCount = 0;
    // delay of the start of playing the music in beats
    // DELAY MUST BE GREATER THAN BEATSSHOWNINADVANCE
    public int delayOfSong = 5000; // millis
    public double millisecondsPerBeat;
    public double millisecondsInAdvance;

    [SerializeField] GameObject noteSpawner;
    [SerializeField] GameObject fretBoardDrawer;
    [SerializeField] GameObject gameManagerGo;
    private GameManager gameManager;

    // NOTE SPECIFIC STUFF
    float spawnHeight;
    float fingerBoardHeight;
    float removeHeight;
    public float noteFallLerpPercent;
    private string[] noteNames;

    public MidiFilePlayer mfp;
    List<MPTKEvent> noteList;
    private MidiLoad ml;
    private double dspTimePrev = 0;

    private void Awake()
    {
        spawnHeight = GameManager.Instance.spawnHeight;
        fingerBoardHeight = GameManager.Instance.fingerBoardHeight;
        removeHeight = GameManager.Instance.removeHeight;
        noteNames = GameManager.Instance.noteNames;
        beatsShownInAdvance = GameManager.Instance.beatsShownInAdvance;

        mfp = FindAnyObjectByType<MidiFilePlayer>();


    }

    //void StartAudio()
    //{

    //}

    void Start()
    {

        //gameManager = gameManagerGo.GetComponent<GameManager>();
        //spawnHeight = gameManager.spawnHeight;
        //fingerBoardHeight = gameManager.fingerBoardHeight;
        //removeHeight = gameManager.removeHeight;
        //noteNames = gameManager.noteNames;
        GetMidiNoteList();
        ml = mfp.MPTK_Load();
        timeSignatureNumerator = ml.MPTK_TimeSigNumerator;
        timeSignatureDenominator = ml.MPTK_NumberQuarterBeat;
        songBpm = ml.MPTK_InitialTempo;
        mfp.MPTK_StartPlayAtFirstNote = true;
        mfp.OnEventNotesMidi.AddListener(NotesToPlay);
        

        //Debug.Log($"SongBPM: {songBpm} or {mfp.MPTK_Tempo} TimeSignature: {timeSignatureNumerator} / {timeSignatureDenominator}");
        // PRINTING MIDI EVENTS
        //List<MPTKEvent> evs = mfp.MPTK_ReadMidiEvents();

        //for (int i = 0; i < 50; ++i)
        //{
        //    Debug.Log(evs[i]);
        //}

        //SetupBeatsToPlay();

        // important for knowing when the note needs to pass the fingerbutton
        // since we want to continue LERPing the note pass the fingerboard
        noteFallLerpPercent = (spawnHeight - fingerBoardHeight) / (spawnHeight - removeHeight);

        // calculate the number of seconds in each beat
        secondsPerBeat = 60 / songBpm;
        Debug.Log($"Seconds Per Beat {secondsPerBeat}");
        notesPerSecond = songBpm / 60f;
        secondsPerMeasure = timeSignatureNumerator * secondsPerBeat;
        millisecondsPerBeat = secondsPerBeat * 1000;

        secondsShownInAdvance = beatsShownInAdvance * secondsPerBeat;
        millisecondsInAdvance = secondsShownInAdvance * 1000;
        //Debug.Log(secondsShownInAdvance);
        //StartAudio();

        // get time as the game loads in before the song starts
        initDspSongTime = AudioSettings.dspTime * 1000;
    }

    void Update()
    {
        Debug.Log($"Current Time: {dspSongTime}");
        // check if song is playing
        // if not playing
        if (!mfp.MPTK_IsPlaying && songPlaying)
        {
            Debug.Log($"DONE: {dspSongTime - delayOfSong}");
        } else if (!mfp.MPTK_IsPlaying) {

            // check if time to play
            if (dspSongTime >= delayOfSong)
            {
                mfp.MPTK_Play();
                songPlaying = true;
            }
        }

        // get the accurate time since game started - time game loaded
        dspSongTime = AudioSettings.dspTime * 1000 - initDspSongTime;
        if (dspTimePrev == dspSongTime)
        {
            dspSongTime += Time.unscaledDeltaTime;
        } else
        {
            dspTimePrev = dspSongTime;
        }

        // spawn lines on fretboard
        if (millisecondsPerBeat * beatCount <= dspSongTime + millisecondsInAdvance)
        {
            Debug.Log($"Spawning Beat for : {dspSongTime}");
            //Debug.Log($"On beat {(timeSignatureNumerator * secondsPerBeat) * beatCount}");
            fretBoardDrawer.GetComponent<FretBoardDrawer>().SpawnMeaureLine(beatCount, beatCount % timeSignatureNumerator == 0);
            beatCount++;
        }
        // determine beats since song started
        //songPositionInBeats = ;
        //Debug.Log($"NextNote Time : {noteList[nextIndex].RealTime / 1000}");
        if (nextIndex < noteList.Count && noteList[nextIndex].RealTime <= dspSongTime + millisecondsInAdvance - delayOfSong)
        {
            Debug.Log($"Spawning Note for : {dspSongTime}");
            noteSpawner.GetComponent<NoteSpawner>().SpawnNote(
                new Vector3(0, spawnHeight, 1),
                new Vector3(0, removeHeight, 1),
                beatsShownInAdvance,
                noteList[nextIndex].RealTime,
                0
                );

            nextIndex++;
        }

        // spawn notes
        //if (nextIndex < notes.Length && notes[nextIndex].beat <= songPositionInBeats + beatsShownInAdvance)
        //{
        //    //Instantiate(music note)
        //    noteSpawner.GetComponent<NoteSpawner>().SpawnNote(
        //        new Vector3(1, spawnHeight, 1),
        //        new Vector3(1, removeHeight, 1),
        //        beatsShownInAdvance,
        //        notes[nextIndex].beat,
        //        notes[nextIndex].noteIndex
        //        );

        //    // Init the fields of the music note
        //    nextIndex++;
        //}

    }

    private void CalculateBeats()
    {

    }


    // This method will be called by the MIDI sequencer just before the notes
    // are playing by the MIDI synthesizer (if 'Send To Synth' is enabled)
    public void NotesToPlay(List<MPTKEvent> mptkEvents)
    {
        //Debug.Log("Received " + mptkEvents.Count + " MIDI Events");
        // Loop on each MIDI events
        foreach (MPTKEvent mptkEvent in mptkEvents)
        {
            // Log if event is a note on
            if (mptkEvent.Command == MPTKCommand.NoteOn)
                //Debug.Log($"Note on Tick:{mptkEvent.Tick}  Note:{mptkEvent.Value} Time:{mptkEvent.RealTime} millis  Velocity:{mptkEvent.Velocity}");
                Debug.Log($"Note Actually Played at Time: {mptkEvent.RealTime} and songTime is {dspSongTime - delayOfSong}");

            // Uncomment to display all MIDI events
            // Debug.Log(mptkEvent.ToString());
        }
    }

    public void GetMidiNoteList()
    {
        noteList = new List<MPTKEvent>();

        // Open and load the Midi
        if (mfp.MPTK_Load() != null)
        {
            // Read midi event to a List<>
            List<MPTKEvent> mptkEvents = mfp.MPTK_ReadMidiEvents();

            // Loop on each Midi events
            foreach (MPTKEvent mptkEvent in mptkEvents)
            {
                // Log if event is a note on
                if (mptkEvent.Command == MPTKCommand.NoteOn)
                {
                    //Debug.Log($"Note on Time:{mptkEvent.RealTime} millisecond  Note:{mptkEvent.Value}  Duration:{mptkEvent.Duration} millisecond  Velocity:{mptkEvent.Velocity}");
                    noteList.Add(mptkEvent);
                }

                // Uncomment to display all Midi events
                //Debug.Log(mptkEvent.ToString());
            }
        }

    }

    public void OnCurrentTick()
    {
        Debug.Log($"CURRENT TICK ON CLICK: {songPosition} CURRENT TIME ON CLICK: {mfp.MPTK_RealTime}");
    }



    // don't have to clutter the rest of the file
    void SetupBeatsToPlay()
    {
        notes = new NoteInformation[52];

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

        // alternating GREEN + YELLOW and RED
        notes[noteCounter++] = new NoteInformation(37.5f, 0);
        notes[noteCounter++] = new NoteInformation(37.5f, 4);
        notes[noteCounter++] = new NoteInformation(38f, 1);
    }
}
