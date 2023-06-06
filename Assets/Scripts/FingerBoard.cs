using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerBoard : MonoBehaviour
{
    private GameObject[] fingerButtons;
    private int numberOfFingerButtons = 4;
    private Vector3[] positions = {
        new Vector3(-4.5f, -4, 0),
        new Vector3(-1.5f, -4, 0),
        new Vector3(1.5f, -4, 0),
        new Vector3(4.5f, -4, 0)
    };

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


    [SerializeField] Sprite fingerButtonSprite;
    [SerializeField] GameObject fingerButton;

    // Start is called before the first frame update
    void Start()
    {
        fingerButtons = new GameObject[numberOfFingerButtons];
        // init the buttons
        for (int i = 0; i < numberOfFingerButtons; ++i)
        {
            fingerButtons[i] = new GameObject("FingerButton" + i);
            fingerButtons[i].AddComponent<FingerButton>();
            fingerButtons[i].AddComponent<SpriteRenderer>();
            fingerButtons[i].GetComponent<SpriteRenderer>().color = colors[i];
            fingerButtons[i].GetComponent<SpriteRenderer>().sprite = fingerButtonSprite;
            fingerButtons[i].GetComponent<Transform>().position = positions[i];
            fingerButtons[i].GetComponent<Transform>().localScale = new Vector3(1, 0.75f, 1);
            fingerButtons[i].GetComponent<Transform>().transform.parent = transform;
            fingerButtons[i].GetComponent<FingerButton>().color = colors[i];
            fingerButtons[i].GetComponent<FingerButton>().key = keyCodes[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfFingerButtons; ++i)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                fingerButtons[i].GetComponent<FingerButton>().onKeyDown(); ;
            }

            if (Input.GetKeyUp(keyCodes[i]))
            {
                fingerButtons[i].GetComponent<FingerButton>().onKeyUp(); ;
            }
        }
    }
}
