using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerButton : MonoBehaviour
{
    public float colorChangeAmount = .2f;
    public Color color { set; get; }
    public KeyCode key { set; get; }

    public float noteHitRange;

    public GameObject outerFingerButton;

    public GameObject noteSpawnerGo;
    private NoteSpawner noteSpawner;

    public GameObject conductorGo;
    private Conductor conductor;

    public GameObject gameManagerGo;
    private GameManager gameManager;

    public Sprite circleSprite;

    private Color outerColor;

    private void Start()
    {
        conductor = conductorGo.GetComponent<Conductor>();
        gameManager = gameManagerGo.GetComponent<GameManager>();
        noteSpawner = noteSpawnerGo.GetComponent<NoteSpawner>();

        outerColor = gameManager.outerNoteColor;
        noteHitRange = gameManager.noteHitRange;

        CreateOuterButton();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            OnKeyDown();
        }

        if (Input.GetKeyUp(key))
        {
            OnKeyUp();
        }
    }

    void CreateOuterButton()
    {
        outerFingerButton = new GameObject(transform.name + " outer");
        outerFingerButton.AddComponent<SpriteRenderer>();
        SpriteRenderer outerSpriteRenderer = outerFingerButton.GetComponent<SpriteRenderer>();
        Transform outerTransform = outerFingerButton.GetComponent<Transform>();

        outerSpriteRenderer.color = outerColor;
        outerSpriteRenderer.sprite = circleSprite;

        outerTransform.position = transform.position;
        outerTransform.localScale = new Vector3(transform.localScale.x * 1.25f, transform.localScale.y * 1.25f, 1);
        outerTransform.parent = transform;
    }

    public void OnKeyDown()
    {
        outerFingerButton.GetComponent<SpriteRenderer>().color = color;
        GetComponent<SpriteRenderer>().color = outerColor;

        if (!DetectRange())
        {
            Debug.Log("YOU MISSED");
            gameManager.OnMissClick();
        } else
        {
            gameManager.OnNoteHit();
        }
    }

    public void OnKeyUp()
    {
        outerFingerButton.GetComponent<SpriteRenderer>().color = outerColor;
        GetComponent<SpriteRenderer>().color = color;
    }

    public bool DetectRange()
    {
        //Note[] notes = noteSpawner.GetComponentInChildren<Note>();
        // check to see if collided with a note of the right color
        foreach (Transform child in noteSpawnerGo.transform)
        {
            Note note = child.GetComponent<Note>();
            if (note.key == key)
            {
                //Debug.Log("Current Beat OnClick: " + conductor.GetComponent<Conductor>().songPositionInBeats);
                //Debug.Log("Note Beat: " + note.name + " " + note.beatOfThisNote);
                //if (Vector3.Distance(transform.position, child.position) < 0.25)
                //{
                //    Destroy(child.gameObject);
                //    return true;
                //} 
                if (Mathf.Abs(conductor.songPositionInBeats - note.beatOfThisNote) <= noteHitRange)
                {
                    Destroy(child.gameObject);
                    return true;
                }
            }
        }
        return false;
    }

}
