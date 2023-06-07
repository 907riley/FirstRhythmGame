using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerButton : MonoBehaviour
{
    public float colorChangeAmount = .2f;
    public Color color { set; get; }
    public KeyCode key { set; get; }

    public float noteHitRange = 0.15f;

    public GameObject noteSpawner;
    public GameObject conductor;

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

    public void OnKeyDown()
    {
        color = new Color(color[0] - colorChangeAmount, color[1] - colorChangeAmount, color[2] - colorChangeAmount, color[3]);
        GetComponent<SpriteRenderer>().color = color;

        if (!DetectRange())
        {
            Debug.Log("YOU MISSED");
        }
    }

    public void OnKeyUp()
    {
        color = new Color(color[0] + colorChangeAmount, color[1] + colorChangeAmount, color[2] + colorChangeAmount, color[3]);
        GetComponent<SpriteRenderer>().color = color;
    }

    public bool DetectRange()
    {
        //Note[] notes = noteSpawner.GetComponentInChildren<Note>();
        // check to see if collided with a note of the right color
        foreach (Transform child in noteSpawner.transform)
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
                if (Mathf.Abs(conductor.GetComponent<Conductor>().songPositionInBeats - note.beatOfThisNote) <= noteHitRange)
                {
                    Destroy(child.gameObject);
                    return true;
                }
            }
        }
        return false;
    }

}
