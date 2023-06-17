using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ----- SETTING FIELDS -----
    // the fields that need to be set manually for the game to run

    // number of fingerbuttons
    public int numberOfFingerButtons = 6;

    // the spacing of the notes/fingerbuttons when they're played
    public float noteSpacingXAxis = 2f;

    // colors of fingerbuttons
    public Color[] colors = {
        new Color(0.87988f, 0, 1f, 1f),
        new Color(0.2074137f, 0.745283f, 0.3176185f, 1),
        new Color(0.7830189f, 0.1514329f, 0.1514329f, 1),
        new Color(0.8553239f, 0.8584906f, 0.2065237f, 1),
        new Color(0.1213955f, 0.2862892f, 0.8301887f, 1),
        new Color(1, 0.4744691f, 0.1745283f, 1)
    };

    // keycodes for fingerbuttons
    public KeyCode[] keyCodes =
    {
        KeyCode.S,
        KeyCode.D,
        KeyCode.F,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L
    };
    
    // error allowed on note hits (beat diff) in milliseconds
    public float noteHitRange = 150f;

    // height of fingerboard
    public float fingerBoardHeight = -4f;

    // the spawn height for notes and measures
    public float spawnHeight = 3f;

    // the remove height for notes and measures
    public float removeHeight = -6f;

    // percent of the size of the fretboard width for the spawn
    public float spawnWidthPercent = 0.75f;

    // amount to scale the notes down at the start
    public float noteSpawnScaleFloat = 0.75f;

    // amount to scale the note down in the Y axis to make it oblong
    public float noteSpawnScaleY = 0.75f;

    // how many measures to play in advance
    public float beatsShownInAdvance = 2f;

    // the actual names of the notes
    public string[] noteNames = 
    {
        "FUSCHIA",
        "GREEN",
        "RED",
        "YELLOW",
        "BLUE",
        "ORANGE"
    };

    // the outer note color, default black
    public Color outerNoteColor = new Color(0, 0, 0, 1);

    // ----- FIELDS THAT ARE DETERMINED AT RUNTIME BY THE SET FIELDS ABOVE -----
    // basically just helper fields so they don't have to be comnputed everytime

    // when to remove notes/measures on the Y scale
    public float deadZoneYAxis;

    // full vector for each note scale at spawn
    public Vector3 noteSpawnScale;

    // the percent of the way down the fretboard where the note should be played
    // or the measure line should intersect the fingerboard
    public float noteFallLerpPercent;

    // the total vertical travel distance for the notes/measures
    public float noteVerticalTravelDistance;

    // the selected song name chosen by the SongSelection scene
    public string selectedSongName;

    // vector positions of fingerbuttons
    public Vector3[] positions;

    // X axis positions of finger buttons
    public float[] fingerButtonXAxis;

    // ----- GAME OBJECTS TO SET TRANSFORM POSITIONS -----
    [SerializeField] GameObject noteSpawnerGo;
    [SerializeField] GameObject fingerBoardGo;
    [SerializeField] GameObject fretBoardDrawerGo;

    // the singleton instance
    public static GameManager Instance;

    private void Awake()
    {
        // Delete GameManager copies
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        noteVerticalTravelDistance = Mathf.Abs(spawnHeight - fingerBoardHeight) + 2;

        deadZoneYAxis = -Mathf.Abs(spawnHeight - removeHeight);
        noteSpawnScale = new Vector3(noteSpawnScaleFloat, noteSpawnScaleFloat * noteSpawnScaleY, 1f);

        // important for knowing when the note needs to pass the fingerbutton
        // since we want to continue LERPing the note pass the fingerboard
        noteFallLerpPercent = (spawnHeight - fingerBoardHeight) / (spawnHeight - removeHeight);

        CalculateFingerButtonXAxis();
        CreatePositions();
    }

    /// <summary>
    /// Called when the settings scene changes the settings. Tells GameManager to reset the settings
    /// </summary>
    /// <param name="fingerButtons"> the number of fingerbuttons </param>
    public void SetNumberOfFingerButtons(int fingerButtons)
    {
        numberOfFingerButtons = fingerButtons;
        CalculateFingerButtonXAxis();
        CreatePositions();
    }

    /// <summary>
    /// Called when the settings scene changes the settings. Tells GameManager to reset the settings
    /// </summary>
    /// <param name="beatsShown"> Beats shown in advance </param>
    public void SetBeatsShownInAdvance(float beatsShown)
    {
        beatsShownInAdvance = beatsShown;
    }

    /// <summary>
    /// Calculates the X axis positions of the fingerbuttons based on the settings
    /// </summary>
    void CalculateFingerButtonXAxis()
    {
        fingerButtonXAxis = new float[numberOfFingerButtons];

        fingerButtonXAxis[0] = -((numberOfFingerButtons - 1) * noteSpacingXAxis / 2);

        for (int i = 1; i < numberOfFingerButtons; ++i)
        {
            fingerButtonXAxis[i] = fingerButtonXAxis[i - 1] + noteSpacingXAxis;
        }
    }

    /// <summary>
    ///  Calculates the vector positions of the fingerbuttons based on the settings
    /// </summary>
    void CreatePositions()
    {
        positions = new Vector3[numberOfFingerButtons];
        for (int i = 0; i < numberOfFingerButtons; ++i)
        {
            positions[i] = new Vector3(fingerButtonXAxis[i], fingerBoardHeight, 0);
        }
    }
}
