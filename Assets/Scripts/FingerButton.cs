using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerButton : MonoBehaviour
{
    public float colorChangeAmount = .2f;
    public Color color { set; get; }
    public KeyCode key { set; get; }

    public void onKeyDown()
    {
        color = new Color(color[0] - colorChangeAmount, color[1] - colorChangeAmount, color[2] - colorChangeAmount, color[3]);
        GetComponent<SpriteRenderer>().color = color;
    }

    public void onKeyUp()
    {
        color = new Color(color[0] + colorChangeAmount, color[1] + colorChangeAmount, color[2] + colorChangeAmount, color[3]);
        GetComponent<SpriteRenderer>().color = color;
    }

}
