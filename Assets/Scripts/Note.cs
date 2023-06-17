using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    // fields set by noteSpawner on creation of Note
    public KeyCode key;
    public Color color;
    public Vector3 spawnPosition;
    public Vector3 removePosition;
    public float beatOfThisNote;
    public Conductor conductor;

    // fields set by the GameManager
    public float deadZoneYAxis;
    private Vector3 noteSpawnScale;
    private float noteSpawnScaleY;

    // songPosition in milliseconds
    public double songPosition;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        // information from the GameManager
        deadZoneYAxis = GameManager.Instance.deadZoneYAxis;
        noteSpawnScaleY = GameManager.Instance.noteSpawnScaleY;
        noteSpawnScale = GameManager.Instance.noteSpawnScale;
        transform.localScale = noteSpawnScale;

        spriteRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = conductor.dspSongTime - conductor.delayOfSong;
        float percentOfTravel = (float)((conductor.millisecondsInAdvance - (beatOfThisNote - songPosition)) / conductor.millisecondsInAdvance * GameManager.Instance.noteFallLerpPercent);

        // interpolate so that it is perfectly in sync
        // so the big part is (beatOfThisNote - songPositionInBeats)
        // when that is positive (beatOfThisNote > sonPositionInBeats)
        //      then we are still approaching the time to play it
        // when this is 0, then we have reached the note and its the remove point
        if (beatOfThisNote <= songPosition + conductor.millisecondsInAdvance)
        {
            transform.position = Vector3.Lerp(
                spawnPosition,
                removePosition,
                percentOfTravel
                );

            // Scaling the notes for depth
            transform.localScale = Vector3.Lerp
                (
                noteSpawnScale,
                new Vector3(1, noteSpawnScaleY, 1),
                percentOfTravel
                );


            // Remove notes if within 0.1 of remove distance
            if (Vector3.Distance(removePosition, transform.position) < 0.1)
            {
                ScoreManager.Instance.OnMissedNote();
                Destroy(gameObject);
            }
        } 
    }
}
