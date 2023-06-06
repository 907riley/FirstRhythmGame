using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] GameObject Note;
    [SerializeField] float bpm = 174;
    [SerializeField] float speed = 4;
    [SerializeField] float spawnRate = 5f;
    [SerializeField] GameObject conductor;

    private float timer = 0f;
    public float[] fingerButtonXAxis = { -4.5f, -1.5f, 1.5f, 4.5f };
    public int noteCounter = 0;

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

    public void SpawnNote(Vector3 spawnPosition, Vector3 removePosition, float beatsShownInAdvance, float beatsOfThisNote, GameObject goConductor)
    {
        int noteIndex = Random.Range(0, fingerButtonXAxis.Length);

        GameObject go = Instantiate(Note, new Vector3(fingerButtonXAxis[noteIndex], spawnPosition.y, 0), transform.rotation);
        // set the parent to be the NoteSpawner so we can find the notes easier
        go.transform.parent = transform;
        go.name = noteNames[noteIndex] + "_" + noteCounter;

        // set the vars in the script
        Note newNote = go.GetComponent<Note>();
        newNote.color = colors[noteIndex];
        newNote.key = keyCodes[noteIndex];
        newNote.spawnPosition = new Vector3(fingerButtonXAxis[noteIndex], 5, 0);
        newNote.removePosition = new Vector3(fingerButtonXAxis[noteIndex], -4, 0);
        newNote.beatsShownInAdvance = beatsShownInAdvance;
        newNote.beatOfThisNote = beatsOfThisNote;
        newNote.go = goConductor;

        ++noteCounter;
    }
}
