using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerButton : MonoBehaviour
{
    // information from GameManager
    public float noteHitRange;
    private Color outerColor;

    // all fields passed on creation from FingerBoard
    public Color color;
    public KeyCode key;
    public GameObject noteSpawnerGo;
    public Conductor conductor;
    public Sprite circleSprite;

    // the outline of the fingerbutton
    public GameObject outerFingerButton;


    private void Start()
    {
        outerColor = GameManager.Instance.outerNoteColor;
        noteHitRange = GameManager.Instance.noteHitRange;

        CreateOuterButton();
    }

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

    /// <summary>
    /// Creates the outer button gameobject
    /// </summary>
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

    /// <summary>
    /// Called when the key associated with the fingerbutton is pressed down
    /// </summary>
    public void OnKeyDown()
    {
        outerFingerButton.GetComponent<SpriteRenderer>().color = color;
        GetComponent<SpriteRenderer>().color = outerColor;

        if (!DetectRange())
        {
            Debug.Log("YOU MISSED");
            ScoreManager.Instance.OnMissClick();
        } else
        {
            ScoreManager.Instance.OnNoteHit();
        }
    }

    /// <summary>
    /// Called when the key associated with the fingerbutton is released
    /// </summary>
    public void OnKeyUp()
    {
        outerFingerButton.GetComponent<SpriteRenderer>().color = outerColor;
        GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// Helper function for detecting if a note was played on time
    /// TODO: make better, super slow rn
    /// </summary>
    /// <returns> true if note is in range, false otherwise </returns>
    public bool DetectRange()
    {
        // check to see if collided with a note of the right color
        foreach (Transform child in noteSpawnerGo.transform)
        {
            Note note = child.GetComponent<Note>();
            if (note.key == key)
            {
                Debug.Log($"Current songposition on click {conductor.dspSongTime - conductor.delayOfSong} compared to note on RealTime: {note.beatOfThisNote}");
                if (Mathf.Abs((float)(conductor.dspSongTime - conductor.delayOfSong - note.beatOfThisNote)) <= noteHitRange)
                {
                    Destroy(child.gameObject);
                    return true;
                }
            }
        }
        return false;
    }
}
