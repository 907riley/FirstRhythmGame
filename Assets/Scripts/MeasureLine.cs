using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureLine : MonoBehaviour
{
    public double songPosition;

    public Vector3 spawnPosition;
    public Vector3 removePosition;
    public int measureCount;
    public float fretboardScale;
    private Vector3 startScale;

    public Conductor conductor;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log($"Start: {spawnPosition.y} Remove: {removePosition.y} MeasureCount: {measureCount}");
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = conductor.dspSongTime;
        float percentOfTravel = (float)((conductor.millisecondsInAdvance - ((conductor.millisecondsPerBeat * measureCount) - songPosition)) / conductor.millisecondsInAdvance * conductor.noteFallLerpPercent);

        if (conductor.millisecondsPerBeat * measureCount <= songPosition + conductor.millisecondsInAdvance)
        {
            transform.position = Vector3.Lerp(
                spawnPosition,
                removePosition,
                //(float)((songPosition - (conductor.secondsPerBeat * measureCount)) / conductor.secondsPerBeat * conductor.noteFallLerpPercent) / GameManager.Instance.beatsShownInAdvance
                percentOfTravel
                );
            //Debug.Log($"MEASURE LINE {transform.name}: {(float)((songPosition - (conductor.secondsPerBeat * measureCount)) / conductor.secondsPerBeat)}")
        }

        // Scaling the notes for depth
        //float percentOfTravel = (float)((songPosition - (conductor.secondsPerBeat * measureCount)) / conductor.secondsPerBeat * conductor.noteFallLerpPercent) / GameManager.Instance.beatsShownInAdvance;
        transform.localScale = Vector3.Lerp
            (
            startScale,
            new Vector3(fretboardScale, 1, 1),
            percentOfTravel
            );

        // Remove notes if within 0.1 of remove distance
        if (Vector3.Distance(removePosition, transform.position) < 0.1)
        {
            //Debug.Log("Missed: " + name);
            //gameManager.OnMissedNote();
            //GameManager.Instance.OnMissedNote();
            //ScoreManager.Instance.OnMissedNote();
            Destroy(gameObject);
        }
    }


}
