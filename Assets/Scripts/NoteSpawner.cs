using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject Note;
    [SerializeField] GameObject conductorGo;

    [SerializeField] GameObject gameManagerGo;
    private GameManager gameManager;

    public float[] fingerButtonXAxis = { -4.5f, -1.5f, 1.5f, 4.5f };
    public float[] removeNoteXAxis;
    public int noteCounter = 0;
    public float noteVerticalTravelDistance = 0;

    private Color[] colors = {
        new Color(0.2074137f, 0.745283f, 0.3176185f, 1),
        new Color(0.7830189f, 0.1514329f, 0.1514329f, 1),
        new Color(0.8553239f, 0.8584906f, 0.2065237f, 1),
        new Color(0.1213955f, 0.2862892f, 0.8301887f, 1),
    };

    private KeyCode[] keyCodes =
    {
        KeyCode.D,
        KeyCode.F,
        KeyCode.J,
        KeyCode.K
    };


    private string[] noteNames =
    {
        "GREEN",
        "RED",
        "YELLOW",
        "BLUE"
    };


    // Start is called before the first frame update
    void Start()
    {
        //spawnRate = bpm / 60 / speed;
        //SpawnNote();
        gameManager = gameManagerGo.GetComponent<GameManager>();
        // init to same size of number of buttons
        removeNoteXAxis = new float[fingerButtonXAxis.Length];
        // TODO: make this real
        noteVerticalTravelDistance = transform.position.y + 1;

        // for moving notes at an angle for depth
        CalculateRemoveXPositions();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (spawnRate <= timer)
    //    {
    //        SpawnNote();
    //        timer = 0;
    //    } else
    //    {
    //        timer += Time.deltaTime;
    //    }
    //}

    public void SpawnNote(Vector3 spawnPosition, Vector3 removePosition, float beatsShownInAdvance, float beatsOfThisNote)
    {
        int noteIndex = Random.Range(0, fingerButtonXAxis.Length);

        GameObject go = Instantiate(Note, new Vector3(transform.position.x, spawnPosition.y, 0), transform.rotation);
        // set the parent to be the NoteSpawner so we can find the notes easier
        go.transform.parent = transform;
        go.name = noteNames[noteIndex] + "_" + noteCounter;

        // set the vars in the script
        Note newNote = go.GetComponent<Note>();
        newNote.color = colors[noteIndex];
        newNote.key = keyCodes[noteIndex];
        newNote.spawnPosition = new Vector3(transform.position.x, spawnPosition.y, 0);
        newNote.removePosition = new Vector3(removeNoteXAxis[noteIndex], removePosition.y, 0);
        newNote.beatsShownInAdvance = beatsShownInAdvance;
        newNote.beatOfThisNote = beatsOfThisNote;
        newNote.conductorGo = conductorGo;
        newNote.gameManagerGo = gameManagerGo;

        gameManager.OnNoteSpawned();
        ++noteCounter;
        Debug.Log(removeNoteXAxis[noteIndex]);
    }

    private void CalculateRemoveXPositions()
    {
        for (int i = 0; i < fingerButtonXAxis.Length; ++i)
        {
            // Uses radians
            float topAngle = Mathf.Atan(Mathf.Abs(fingerButtonXAxis[i]) / transform.position.y);
            float removeX = noteVerticalTravelDistance * Mathf.Tan(topAngle);
            
            // Moving left to right set negative since at middle
            if (i < fingerButtonXAxis.Length / 2)
            {
                removeNoteXAxis[i] = -removeX;
            } else
            {
                removeNoteXAxis[i] = removeX;
            }
        }
    }
}
