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

    // falling note prefab
    [SerializeField] GameObject note;
    // circle asset
    [SerializeField] Sprite fingerButtonSprite;
    [SerializeField] GameObject noteSpawner;

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

            SpriteRenderer fingerButtonSpriteRenderer = fingerButtons[i].GetComponent<SpriteRenderer>();
            Transform fingerButtonTransform = fingerButtons[i].GetComponent<Transform>();
            FingerButton fingerButtonSelf = fingerButtons[i].GetComponent<FingerButton>();

            fingerButtonSpriteRenderer.color = colors[i];
            fingerButtonSpriteRenderer.sprite = fingerButtonSprite;
            fingerButtonTransform.position = positions[i];
            fingerButtonTransform.localScale = new Vector3(1, 0.75f, 1);
            fingerButtonTransform.transform.parent = transform;
            fingerButtonSelf.color = colors[i];
            fingerButtonSelf.key = keyCodes[i];
            fingerButtonSelf.noteSpawner = noteSpawner;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfFingerButtons; ++i)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                fingerButtons[i].GetComponent<FingerButton>().OnKeyDown(); ;
            }

            if (Input.GetKeyUp(keyCodes[i]))
            {
                fingerButtons[i].GetComponent<FingerButton>().OnKeyUp(); ;
            }
        }
    }
}
