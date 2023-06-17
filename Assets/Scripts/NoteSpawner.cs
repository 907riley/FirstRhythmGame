using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    // prefab
    [SerializeField] GameObject Note;

    [SerializeField] GameObject conductorGo;
    private Conductor conductor;

    // the x axis cords of where to remove notes
    public float[] removeNoteXAxis;

    // helpful note counter
    public int noteCounter = 0;

    // fields set by the GameManager
    public float[] fingerButtonXAxis;
    private Vector3[] positions;
    private Color[] colors;
    private KeyCode[] keyCodes;
    private string[] noteNames;
    private float spawnWidthPercent;
    public float noteVerticalTravelDistance;

    public List<GameObject>[] currentNotes;

    private void Awake()
    {
        conductor = conductorGo.GetComponent<Conductor>();
    }

    void Start()
    {

        transform.position = new Vector3(0, GameManager.Instance.spawnHeight, 0);

        fingerButtonXAxis = GameManager.Instance.fingerButtonXAxis;
        colors = GameManager.Instance.colors;
        keyCodes = GameManager.Instance.keyCodes;
        noteNames = GameManager.Instance.noteNames;
        positions = GameManager.Instance.positions;
        spawnWidthPercent = GameManager.Instance.spawnWidthPercent;
        noteVerticalTravelDistance = GameManager.Instance.noteVerticalTravelDistance;

        // init to same size of number of buttons
        removeNoteXAxis = new float[positions.Length];

        // for moving notes at an angle for depth
        CalculateRemoveXPositions();
    }

    /// <summary>
    /// Function for spawning the note prefab
    /// </summary>
    /// <param name="realTimeOfNote"> The real time in milliseconds of when the note is played </param>
    /// <param name="noteIndex"> the index of the notes (ie green, red, yellow) </param>
    public void SpawnNote(float realTimeOfNote, int noteIndex)
    {

        GameObject go = Instantiate(Note, new Vector3(fingerButtonXAxis[noteIndex] * spawnWidthPercent, GameManager.Instance.spawnHeight, 0), transform.rotation);
        // set the parent to be the NoteSpawner so we can find the notes easier
        go.transform.parent = transform;
        go.name = noteNames[noteIndex] + "_" + noteCounter;

        // set the vars in the script
        Note newNote = go.GetComponent<Note>();
        newNote.spawnPosition = new Vector3(fingerButtonXAxis[noteIndex] * spawnWidthPercent, transform.position.y, 0);
        newNote.removePosition = new Vector3(removeNoteXAxis[noteIndex], GameManager.Instance.removeHeight, 0);
        newNote.beatOfThisNote = realTimeOfNote;
        newNote.conductor = conductor;

        if (GameManager.Instance.numberOfFingerButtons == 4) {
            newNote.color = colors[noteIndex + 1];
            newNote.key = keyCodes[noteIndex + 1];
        } else
        {
            newNote.color = colors[noteIndex];
            newNote.key = keyCodes[noteIndex];
        }

        ScoreManager.Instance.OnNoteSpawned();
        ++noteCounter;
    }

    /// <summary>
    /// Calculates the x positions to remove the notes
    /// Uses trig since they are triangles (off screen)
    /// </summary>
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
