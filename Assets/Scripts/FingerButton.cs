using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerButton : MonoBehaviour
{
    public float colorChangeAmount = .3f;
    public Color color { set; get; }
    public KeyCode key { set; get; }

    public void onKeyDown()
    {
        Debug.Log("Before " + color);
        color = new Color(color[0], color[1] - colorChangeAmount, color[2], color[3]);
        GetComponent<SpriteRenderer>().color = color;
        Debug.Log("After " + color);
    }

    public void onKeyUp()
    {
        color = new Color(color[0], color[1] + colorChangeAmount, color[2], color[3]);
        GetComponent<SpriteRenderer>().color = color;
    }

}
