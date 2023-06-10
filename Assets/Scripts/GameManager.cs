using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ----- FINGERBOARD INFO -----
    public int numberOfFingerButtons = 5;

    // vector positions of fingerbuttons
    public Vector3[] positions;

    // X axis positions of finger buttons
    public float[] fingerButtonXAxis;
    public float noteSpacingXAxis = 2f;

    // colors of fingerbuttons
    public Color[] colors = {
        new Color(0.2074137f, 0.745283f, 0.3176185f, 1),
        new Color(0.7830189f, 0.1514329f, 0.1514329f, 1),
        new Color(0.8553239f, 0.8584906f, 0.2065237f, 1),
        new Color(0.1213955f, 0.2862892f, 0.8301887f, 1),
        new Color(1, 0.4744691f, 0.1745283f, 1)
    };

    // keycodes for fingerbuttons
    public KeyCode[] keyCodes =
    {
        KeyCode.D,
        KeyCode.F,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L
    };
    
    // error allowed on note hits (beat diff)
    public float noteHitRange = 0.5f;

    // height of fingerboard
    public float fingerBoardHeight = -4f;

    // ----- NOTE SPAWN/REMOVE INFO -----
    public float spawnHeight = 3f;
    public float removeHeight = -6f;
    public float deadZoneYAxis;
    // percent of the size of the fretboard width for the spawn
    public float spawnWidthPercent = 0.25f;

    // amount to scale the notes down at the start
    public float noteSpawnScaleFloat = 0.5f;
    // amount to scale the note down in the Y axis to make it oblong
    public float noteSpawnScaleY = 0.75f;
    public Vector3 noteSpawnScale;

    // basically speed of notes
    public float beatsShownInAdvance = 2f;

    public string[] noteNames = 
    {
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

    public Color outerNoteColor = new Color(0, 0, 0, 1);

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
    }

    void Start()
    {
        //noteSpawnerGo.GetComponent<Transform>().position = new Vector3(0, spawnHeight, 0);
        //fingerBoardGo.GetComponent<Transform>().position = new Vector3(0, fingerBoardHeight, 0);
        //fretBoardDrawerGo.GetComponent<Transform>().position = new Vector3(0, spawnHeight, 0);

        deadZoneYAxis = -Mathf.Abs(spawnHeight - removeHeight);
        noteSpawnScale = new Vector3(noteSpawnScaleFloat, noteSpawnScaleFloat * noteSpawnScaleY, 1f);

        CalculateFingerButtonXAxis();
        CreatePositions();
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
