using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureLine : MonoBehaviour
{
    // the current songPosition obtained from the conductor class
    public double songPosition;

    // all information passed by the FretBoardDrawer
    public Vector3 spawnPosition;
    public Vector3 removePosition;
    public int measureCount;
    public float fretboardScale;
    public Conductor conductor;

    // the scale the measureline starts at
    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = conductor.dspSongTime - conductor.delayOfSong;
        float percentOfTravel = (float)((conductor.millisecondsInAdvance - ((conductor.millisecondsPerBeat * measureCount) - songPosition)) / conductor.millisecondsInAdvance * GameManager.Instance.noteFallLerpPercent);

        if (conductor.millisecondsPerBeat * measureCount <= songPosition + conductor.millisecondsInAdvance)
        {
            transform.position = Vector3.Lerp(
                spawnPosition,
                removePosition,
                percentOfTravel
                );
            //Debug.Log($"MEASURE LINE {transform.name}: {(float)((songPosition - (conductor.secondsPerBeat * measureCount)) / conductor.secondsPerBeat)}")
        }

        // Scaling the notes for depth
        transform.localScale = Vector3.Lerp
            (
            startScale,
            new Vector3(fretboardScale, 1, 1),
            percentOfTravel
            );

        // Remove measureline if within 0.1 of remove distance
        if (Vector3.Distance(removePosition, transform.position) < 0.1)
        {
            Destroy(gameObject);
        }
    }
}
