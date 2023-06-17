using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class Conductor : MonoBehaviour
{
    // beats per minute of song
    public double songBpm;

    // seconds per beat of song
    public double secondsPerBeat;

    // miliseconds per beat
    public double millisecondsPerBeat;

    // song position in beats
    public int songPositionInBeats = 0;

    // the initial AudioSettings.dspTime time in milliseconds
    public double initDspSongTime;

    // Seconds passed since song started in milliseconds
    public double dspSongTime = 0;

    // Offset to correct the diff in unitys timing and midi player in milliseconds
    public double offSet = 0f;

    // the index of the next note to spawn
    int nextIndex = 0;

    // the number of beats to show in advance
    public float beatsShownInAdvance;

    // seconds shown in advance
    public double secondsShownInAdvance;

    // milliseconds shown in advance
    public double millisecondsInAdvance;

    // bool to determine if song is playing (helpful for knowing when song is done)
    public bool songPlaying = false;

    // the time signature numerator
    public double timeSignatureNumerator;

    // the time signature denominator
    public double timeSignatureDenominator;

    // delay of the start of playing the music in beats in milliseconds, idealy greater than millisecondsInAdvance
    public int delayOfSong = 5000; 

    // GameObjects defined in the interpreter
    [SerializeField] GameObject noteSpawner;
    [SerializeField] GameObject fretBoardDrawer;
    [SerializeField] GameObject endGamePanel;

    // object to play the midi song
    [SerializeField] MidiFilePlayer mfp;
    // list of notes that need to be played
    List<MPTKEvent> noteList;
    // the midi load from the song that gives us time sig and tempo info
    private MidiLoad ml;

    // function variable to let us define the 6 or 4 finger button function for dividing up notes
    private delegate int IdentifyNoteDel(int noteValue);
    private IdentifyNoteDel identifyNote;


    void Start()
    {
        beatsShownInAdvance = GameManager.Instance.beatsShownInAdvance;
        mfp.MPTK_MidiName = GameManager.Instance.selectedSongName;

        // get all the midi info
        GetMidiNoteList();
        ml = mfp.MPTK_Load();
        timeSignatureNumerator = ml.MPTK_TimeSigNumerator;
        timeSignatureDenominator = ml.MPTK_NumberQuarterBeat;
        songBpm = ml.MPTK_InitialTempo;
        mfp.MPTK_StartPlayAtFirstNote = true;
        // mainly for debugging, notes to play only debug.logs
        mfp.OnEventNotesMidi.AddListener(NotesToPlay);

        // determine the function to use for dividing up notes
        // TODO: add error if identifyNote returns -1
        if (GameManager.Instance.numberOfFingerButtons == 6)
        {
            identifyNote = new IdentifyNoteDel(IdentifyNoteTypeSixFingers);
        } else if (GameManager.Instance.numberOfFingerButtons == 4)
        {
            identifyNote = new IdentifyNoteDel(IdentifyNoteTypeFourFingers);
        }


        // some calculations need later on
        secondsPerBeat = 60 / songBpm;
        millisecondsPerBeat = secondsPerBeat * 1000;

        secondsShownInAdvance = beatsShownInAdvance * secondsPerBeat;
        millisecondsInAdvance = secondsShownInAdvance * 1000;

        // get time as the game loads in before the song starts
        initDspSongTime = AudioSettings.dspTime * 1000;
    }

    void Update()
    {
        // check if song is playing
        // if not playing
        if (!mfp.MPTK_IsPlaying && songPlaying)
        {
            EndOfGame();
        } else if (!mfp.MPTK_IsPlaying) {

            // check if time to play
            if (dspSongTime >= delayOfSong)
            {
                mfp.MPTK_Play();
                songPlaying = true;
            }
        }

        // get the accurate time since game started - time game loaded - unity time inaccuracy
        dspSongTime = AudioSettings.dspTime * 1000 - initDspSongTime - offSet;
        Debug.Log($"Current Time: {dspSongTime}");
        Debug.Log($"Current Time according to mfp: {mfp.MPTK_RealTime}");


        // spawn lines on fretboard
        if (millisecondsPerBeat * songPositionInBeats <= dspSongTime + millisecondsInAdvance - delayOfSong)
        {
            Debug.Log($"Spawning Beat for : {dspSongTime}");
            fretBoardDrawer.GetComponent<FretBoardDrawer>().SpawnMeaureLine(songPositionInBeats, songPositionInBeats % timeSignatureNumerator == 0);
            songPositionInBeats++;
        }
        
        // spawn notes
        if (nextIndex < noteList.Count && noteList[nextIndex].RealTime <= dspSongTime + millisecondsInAdvance - delayOfSong)
        {
            Debug.Log($"Spawning Note for : {dspSongTime}");
            noteSpawner.GetComponent<NoteSpawner>().SpawnNote(
                noteList[nextIndex].RealTime,
                identifyNote(noteList[nextIndex].Value)
                );
            // resetting back to the midi player timing every note to reduce the speed up
            if (nextIndex >= 0)
            {
                offSet = (dspSongTime - delayOfSong + millisecondsInAdvance) - noteList[nextIndex].RealTime;
                Debug.Log($" *** Incrementing Offset to {offSet} by {(dspSongTime - delayOfSong + millisecondsInAdvance)} - {noteList[nextIndex].RealTime}");
            }
            nextIndex++;
        }
    }

    /// <summary>
    /// End of game method, incase we need to add more to this made it a method.
    /// </summary>
    private void EndOfGame()
    {
        endGamePanel.SetActive(true);
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
                Debug.Log($"*** Note Actually Played at Time: {mptkEvent.RealTime} and songTime is {dspSongTime - delayOfSong} or mfp realtime {mfp.MPTK_RealTime}");

            // Uncomment to display all MIDI events
            // Debug.Log(mptkEvent.ToString());
        }
    }

    /// <summary>
    /// Gets the list of NoteOn events from the MPTKEvent list from load
    /// TODO: Get notes such that:
    ///         1. they don't land on top of each other
    ///         2. they are only on beats (or half beats, quarter beats, etc)
    /// </summary>
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
                // Add if event is a note on
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

    /// <summary>
    /// Dividing notes into fingerbuttons based on the note value
    /// For four fingerbuttons
    /// </summary>
    /// <param name="noteNumber"></param>
    /// <returns> the fingerboard index to use for the note </returns>
    public int IdentifyNoteTypeFourFingers(int noteNumber)
    {
        if (noteNumber > 127 || noteNumber < 0)
        {
            return -1;
        } else
        {
            
            int modNumber = noteNumber % 12;
            //Debug.Log($"NoteNumber modded: {modNumber}");
            if (modNumber > 8)
            {
                return 3;
            } else if (modNumber > 5)
            {
                return 2;
            } else if (modNumber > 2)
            {
                return 1;
            } else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Dividing notes into fingerbuttons based on the note value
    /// For six fingerbuttons
    /// </summary>
    /// <param name="noteNumber"></param>
    /// <returns> the fingerboard index to use for the note </returns>
    public int IdentifyNoteTypeSixFingers(int noteNumber)
    {
        if (noteNumber > 127 || noteNumber < 0)
        {
            return -1;
        }
        else
        {
            int modNumber = noteNumber % 12;
            if (modNumber > 9)
            {
                return 5;
            }
            else if (modNumber > 7)
            {
                return 4;
            }
            else if (modNumber > 5)
            {
                return 3;
            }
            else if (modNumber > 3)
            {
                return 2;
            }
            else if (modNumber > 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
