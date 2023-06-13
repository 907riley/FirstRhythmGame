using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public KeyCode key { set; get; }
    public Color color { set; get; }
    public float speed { set; get; }

    public float deadZoneYAxis;

    public Vector3 spawnPosition;
    public Vector3 removePosition;
    public float beatsShownInAdvance;
    public float beatOfThisNote;
    public double songPosition;

    private Vector3 noteSpawnScale;
    private float noteSpawnScaleY;

    public GameObject conductorGo;
    private Conductor conductor;

    public GameObject gameManagerGo;
    private GameManager gameManager;

    private SpriteRenderer spriteRenderer;

    private GameObject outerNote;
    // Start is called before the first frame update
    void Start()
    {
        conductor = conductorGo.GetComponent<Conductor>();
        //gameManager = gameManagerGo.GetComponent<GameManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = 1;

        //noteSpawnScale = gameManager.noteSpawnScale;
        noteSpawnScale = GameManager.Instance.noteSpawnScale;

        transform.localScale = noteSpawnScale;

        //deadZoneYAxis = gameManager.deadZoneYAxis;
        deadZoneYAxis = GameManager.Instance.deadZoneYAxis;
        //CreateOuterButton();
        noteSpawnScaleY = GameManager.Instance.noteSpawnScaleY;
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = conductor.dspSongTime - conductor.delayOfSong;
        float percentOfTravel = (float)((conductor.secondsShownInAdvance - (beatOfThisNote - songPosition)) / conductor.secondsShownInAdvance * conductor.noteFallLerpPercent);

        // interpolate so that it is perfectly in sync
        // beatsShownInAdvance is like speed I think
        // so the big part is (beatOfThisNote - songPositionInBeats)
        // when that is positive (beatOfThisNote > sonPositionInBeats)
        //      then we are still approaching the time to play it
        // when this is 0, then we have reached the note and its the remove point
        if (beatOfThisNote <= songPosition + conductor.secondsShownInAdvance)
        {
            //Debug.Log("Lerp t: " + (beatsShownInAdvance - (beatOfThisNote - songPositionInBeats)) / beatsShownInAdvance * conductor.noteFallLerpPercent + " fallpercent: " + conductor.noteFallLerpPercent);
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

            //outerNote.GetComponent<Transform>().localScale = new Vector3(percentOfTravel + 0.5f, percentOfTravel * .75f + 0.5f, percentOfTravel);

            // Remove notes if within 0.1 of remove distance
            if (Vector3.Distance(removePosition, transform.position) < 0.1)
            {
                Debug.Log("Missed: " + name);
                //gameManager.OnMissedNote();
                //GameManager.Instance.OnMissedNote();
                ScoreManager.Instance.OnMissedNote();
                Destroy(gameObject);
            }
        } 

        //if (transform.position == removePosition)
        //{
        //    Destroy(gameObject);
        //}
    }

    void CreateOuterButton()
    {
        outerNote = new GameObject(transform.name + " outer");
        outerNote.AddComponent<SpriteRenderer>();
        SpriteRenderer outerSpriteRenderer = outerNote.GetComponent<SpriteRenderer>();
        Transform outerTransform = outerNote.GetComponent<Transform>();

        //outerSpriteRenderer.color = gameManager.outerNoteColor;
        outerSpriteRenderer.color = GameManager.Instance.outerNoteColor;
        outerSpriteRenderer.sprite = transform.GetComponent<SpriteRenderer>().sprite;

        outerTransform.position = transform.position;
        outerTransform.localScale = transform.localScale;
        outerTransform.parent = transform;
    }
}
