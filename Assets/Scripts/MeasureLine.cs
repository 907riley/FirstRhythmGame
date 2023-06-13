using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureLine : MonoBehaviour
{
    public double songPosition;

    public Vector3 spawnPosition;
    public Vector3 removePosition;
    public int measureCount;

    public Conductor conductor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = conductor.dspSongTime;

        if (conductor.secondsPerMeasure * measureCount <= songPosition)
        {
            transform.position = Vector3.Lerp(
                spawnPosition,
                removePosition,
                (float)((songPosition - (conductor.secondsPerMeasure * measureCount)) / conductor.secondsPerMeasure * conductor.noteFallLerpPercent)
                );
        }
    }


}
