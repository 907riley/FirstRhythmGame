using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public KeyCode key { set; get; }
    public Color color { set; get; }
    public float speed { set; get; }
    public float deadZoneYAxis = -6f;

    // Start is called before the first frame update
    void Start()
    {
        speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.down;

        if (transform.position.y < deadZoneYAxis)
        {
            Destroy(gameObject);
        }
    }
}
