using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerBoard : MonoBehaviour
{
    [SerializeField] GameObject greenButton;
    [SerializeField] GameObject redButton;
    [SerializeField] GameObject yellowButton;
    [SerializeField] GameObject blueButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            onGreenClick();
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            onGreenRelease();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            onRedClick();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            onYellowClick();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            onBlueClick();
        }
    }

    public void onGreenClick()
    {
        Color color = greenButton.GetComponent<SpriteRenderer>().color;
        color[1] -= .3f;
        greenButton.GetComponent<SpriteRenderer>().color = color;
        Debug.Log("Green Clicked: " + color);
    }

    public void onGreenRelease()
    {
        Color color = greenButton.GetComponent<SpriteRenderer>().color;
        color[1] += .3f;
        greenButton.GetComponent<SpriteRenderer>().color = color;
        Debug.Log("Green Clicked: " + color);
    }

    public void onRedClick()
    {
        Debug.Log("Red Clicked");
    }

    public void onYellowClick()
    {
        Debug.Log("Yellow Clicked");
    }

    public void onBlueClick()
    {
        Debug.Log("Blue Clicked");
    }
}
