using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ----- FINGERBOARD INFO -----
    public int numberOfFingerButtons = 6;

    // vector positions of fingerbuttons
    public Vector3[] positions;

    // X axis positions of finger buttons
    public float[] fingerButtonXAxis;
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
    
    // error allowed on note hits (beat diff)
    public float noteHitRange = 150f;

    // height of fingerboard
    public float fingerBoardHeight = -4f;

    // ----- NOTE SPAWN/REMOVE INFO -----
    public float spawnHeight = 3f;
    public float removeHeight = -6f;
    public float deadZoneYAxis;
    // percent of the size of the fretboard width for the spawn
    public float spawnWidthPercent = 0.75f;

    // amount to scale the notes down at the start
    public float noteSpawnScaleFloat = 0.75f;
    // amount to scale the note down in the Y axis to make it oblong
    public float noteSpawnScaleY = 0.75f;
    public Vector3 noteSpawnScale;

    // basically speed of notes
    public float beatsShownInAdvance = 4f;

    public string[] noteNames = 
    {
        "FUSCHIA",
        "GREEN",
        "RED",
        "YELLOW",
        "BLUE",
        "ORANGE"
    };

    //public enum NoteNames
    //{
    //    GREEN = 0,
    //    RED = 1,
    //    YELLOW = 2,
    //    BLUE = 3
    //};

    //public NoteNames[] notes =
    //{
    //    NoteNames.GREEN,
    //    NoteNames.RED,
    //    NoteNames.YELLOW,
    //    NoteNames.BLUE
    //};

    public float noteFallLerpPercent;

    public float noteVerticalTravelDistance;

    public Color outerNoteColor = new Color(0, 0, 0, 1);

    public string selectedSongName;

    // ----- GAME OBJECTS TO SET TRANSFORM POSITIONS -----
    [SerializeField] GameObject noteSpawnerGo;
    [SerializeField] GameObject fingerBoardGo;
    [SerializeField] GameObject fretBoardDrawerGo;

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

    public void SetNumberOfFingerButtons(int fingerButtons)
    {
        numberOfFingerButtons = fingerButtons;
        CalculateFingerButtonXAxis();
        CreatePositions();
    }

    public void SetBeatsShownInAdvance(float beatsShown)
    {
        beatsShownInAdvance = beatsShown;
    }

    void CalculateFingerButtonXAxis()
    {
        fingerButtonXAxis = new float[numberOfFingerButtons];

        fingerButtonXAxis[0] = -((numberOfFingerButtons - 1) * noteSpacingXAxis / 2);

        for (int i = 1; i < numberOfFingerButtons; ++i)
        {
            fingerButtonXAxis[i] = fingerButtonXAxis[i - 1] + noteSpacingXAxis;
        }
    }

    void CreatePositions()
    {
        positions = new Vector3[numberOfFingerButtons];
        for (int i = 0; i < numberOfFingerButtons; ++i)
        {
            positions[i] = new Vector3(fingerButtonXAxis[i], fingerBoardHeight, 0);
        }
    }
}
