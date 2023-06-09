using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerBoard : MonoBehaviour
{
    private GameObject[] fingerButtons;
    private int numberOfFingerButtons;
    public Vector3[] positions;
    private Color[] colors;
    private KeyCode[] keyCodes;

    // circle asset
    [SerializeField] Sprite fingerButtonSprite;

    [SerializeField] GameObject noteSpawnerGo;
    [SerializeField] GameObject conductorGo;
    [SerializeField] GameObject gameManagerGo;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameManagerGo.GetComponent<GameManager>();

        numberOfFingerButtons = gameManager.numberOfFingerButtons;
        positions = gameManager.positions;
        colors = gameManager.colors;
        keyCodes = gameManager.keyCodes;

        CreateFingerButtons();
    }

    void CreateFingerButtons()
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

            fingerButtonSpriteRenderer.sortingOrder = 1;
            fingerButtonSpriteRenderer.color = colors[i];
            fingerButtonSpriteRenderer.sprite = fingerButtonSprite;
            fingerButtonTransform.position = positions[i];
            fingerButtonTransform.localScale = new Vector3(1, 0.75f, 1);
            fingerButtonTransform.transform.parent = transform;
            fingerButtonSelf.color = colors[i];
            fingerButtonSelf.key = keyCodes[i];
            fingerButtonSelf.noteSpawnerGo = noteSpawnerGo;
            fingerButtonSelf.conductorGo = conductorGo;
            fingerButtonSelf.gameManagerGo = gameManagerGo;
            fingerButtonSelf.circleSprite = fingerButtonSprite;
        }
    }

}
