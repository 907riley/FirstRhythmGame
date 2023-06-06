using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerButton : MonoBehaviour
{
    public float colorChangeAmount = .2f;
    public Color color { set; get; }
    public KeyCode key { set; get; }

    public GameObject noteSpawner;

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
                if (Vector3.Distance(transform.position, child.position) < 0.25)
                {
                    Destroy(child.gameObject);
                    return true;
                } 
            }
        }
        return false;
    }

}
