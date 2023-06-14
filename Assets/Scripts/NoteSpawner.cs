using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject Note;
    [SerializeField] GameObject conductorGo;

    [SerializeField] GameObject gameManagerGo;
    private GameManager gameManager;

    public float[] removeNoteXAxis;
    public int noteCounter = 0;
    public float noteVerticalTravelDistance = 0;

    public float[] fingerButtonXAxis;
    private Vector3[] positions;
    private Color[] colors;
    private KeyCode[] keyCodes;
    private string[] noteNames;
    private float spawnWidthPercent;

    public List<GameObject>[] currentNotes;


    void Start()
    {

        //gameManager = gameManagerGo.GetComponent<GameManager>();

        transform.position = new Vector3(0, GameManager.Instance.spawnHeight, 0);
        //fingerButtonXAxis = gameManager.fingerButtonXAxis;
        //colors = gameManager.colors;
        //keyCodes = gameManager.keyCodes;
        //noteNames = gameManager.noteNames;
        //positions = gameManager.positions;
        //spawnWidthPercent = gameManager.spawnWidthPercent;

        fingerButtonXAxis = GameManager.Instance.fingerButtonXAxis;
        colors = GameManager.Instance.colors;
        keyCodes = GameManager.Instance.keyCodes;
        noteNames = GameManager.Instance.noteNames;
        positions = GameManager.Instance.positions;
        spawnWidthPercent = GameManager.Instance.spawnWidthPercent;
        //InitCurrentNoteLists();


        // init to same size of number of buttons
        removeNoteXAxis = new float[positions.Length];
        // TODO: make this real
        noteVerticalTravelDistance = Mathf.Abs(transform.position.y - positions[0].y) + 2;

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
    //private void InitCurrentNoteLists()
    //{
    //    currentNotes = new List<GameObject>[noteNames.Length];

    //    for (int i = 0; i  < noteNames.Length; ++i)
    //    {
    //        currentNotes[i] = new List<GameObject>();
    //    }
    //}

    // TODO: RENAME EVERYTHING, REMOVE SPAWN AND REMOVE
    public void SpawnNote(Vector3 spawnPosition, Vector3 removePosition, float beatsShownInAdvance, float beatsOfThisNote, int noteIndex)
    {

        GameObject go = Instantiate(Note, new Vector3(fingerButtonXAxis[noteIndex] * spawnWidthPercent, spawnPosition.y, 0), transform.rotation);
        // set the parent to be the NoteSpawner so we can find the notes easier
        go.transform.parent = transform;
        go.name = noteNames[noteIndex] + "_" + noteCounter;

        // set the vars in the script
        Note newNote = go.GetComponent<Note>();
        newNote.spawnPosition = new Vector3(fingerButtonXAxis[noteIndex] * spawnWidthPercent, transform.position.y, 0);
        newNote.removePosition = new Vector3(removeNoteXAxis[noteIndex], removePosition.y, 0);
        newNote.beatsShownInAdvance = beatsShownInAdvance;
        newNote.beatOfThisNote = beatsOfThisNote;
        newNote.conductorGo = conductorGo;
        newNote.gameManagerGo = gameManagerGo;

        if (GameManager.Instance.numberOfFingerButtons == 4) {
            newNote.color = colors[noteIndex + 1];
            newNote.key = keyCodes[noteIndex + 1];
        } else
        {
            newNote.color = colors[noteIndex];
            newNote.key = keyCodes[noteIndex];
        }

        // add note object to the currentNotesList
        //currentNotes[noteIndex].Add(go);

        //gameManager.OnNoteSpawned();
        //GameManager.Instance.OnNoteSpawned();
        ScoreManager.Instance.OnNoteSpawned();
        ++noteCounter;
        //Debug.Log(removeNoteXAxis[noteIndex]);
    }

    private void CalculateRemoveXPositions()
    {
        for (int i = 0; i < positions.Length; ++i)
        {
            // Uses radians
            float topAngle = Mathf.Atan(Mathf.Abs(positions[i].x - positions[i].x * spawnWidthPercent) / Mathf.Abs(transform.position.y - positions[i].y));
            float removeX = noteVerticalTravelDistance * Mathf.Tan(topAngle);
            
            // Moving left to right set negative since at middle
            if (i < positions.Length / 2)
            {
                removeNoteXAxis[i] = -removeX + positions[i].x * spawnWidthPercent;
            } else
            {
                removeNoteXAxis[i] = removeX + positions[i].x * spawnWidthPercent;
            }
        }
    }

}
