using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FretBoardDrawer : MonoBehaviour
{
    // interpreter fields
    [SerializeField] Material fullMeasureLineMaterial;
    [SerializeField] Material lineMaterial;
    [SerializeField] Material fretBoardMaterial;
    [SerializeField] GameObject fingerBoardGo;
    private FingerBoard fingerBoard;

    [SerializeField] GameObject conductorGo;
    private Conductor conductor;

    // the vertices inbetween the fingerButtons where the lines should go for lanes
    private Vector3[] inbetweenFingerBoard;

    // how far the lines go vertically, for triangles
    private float noteVerticalTravelDistance;

    // the vertices at the top of fretboard where the lines should start
    private Vector3[] topInbetweenLocations;

    private float spawnWidthPercent;
    private float spawnHeight;

    private Mesh fretMesh;
    private MeshFilter fretMeshFilter;
    private MeshRenderer meshRenderer;

    private GameObject topLine;

    // the scale of the bottom of the fretboard
    // from the top of the fretboard
    public float fretboardScale;

    private void Awake()
    {
        fingerBoard = fingerBoardGo.GetComponent<FingerBoard>();
        conductor = conductorGo.GetComponent<Conductor>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, GameManager.Instance.spawnHeight, 0);
        noteVerticalTravelDistance = GameManager.Instance.noteVerticalTravelDistance;
        spawnWidthPercent = GameManager.Instance.spawnWidthPercent;
        spawnHeight = GameManager.Instance.spawnHeight;

        // the points where the fretboard vertical lines should intersect the fingerboard
        FindInbetweenLocations();

        // the points where the fretboard verticals should start at the top of the fretboard
        FindTopInbetweenLocation();

        // the points where the fretboard vertical lines should end off screen
        CalculateRemoveXPositions();

        // draw the fretboard background
        DrawFretBackground();

        // draw all the lines as gameobjects
        for (int i = 0; i < inbetweenFingerBoard.Length; ++i)
        {
            GameObject line = new GameObject();
            line.name = $"vertical_line_{i}";
            DrawLine(topInbetweenLocations[i], inbetweenFingerBoard[i], new Color(0.6f, 0.7f, 0.2f, 1), line);
        }

        // make the top line
        topLine = new GameObject();
        topLine.name = $"top_line";
        DrawLine(topInbetweenLocations[0], topInbetweenLocations[^1], new Color(0, 0, 0, 1), topLine);

        fretboardScale =  Mathf.Abs(inbetweenFingerBoard[0].x - inbetweenFingerBoard[^1].x)/Mathf.Abs(topInbetweenLocations[0].x - topInbetweenLocations[^1].x);
    }

    /// <summary>
    /// method for spawning the measure lines at the top of the fretboard
    /// </summary>
    /// <param name="measureCount"> the measure number (ie the number of measures spawned </param>
    /// <param name="measureLine"> true means start/end of full measure </param>
    public void SpawnMeaureLine(int measureCount, bool measureLine)
    {
        GameObject go = Instantiate(topLine, transform);

        // set the parent to be the FretBoard so we can find the notes easier
        go.transform.parent = transform;
        go.name = $"measure_line_{measureCount}";
        go.GetComponent<LineRenderer>().useWorldSpace = false;

        // set the vars in the script
        MeasureLine measure = go.AddComponent<MeasureLine>();

        measure.spawnPosition = new Vector3(0, 0, 0);
        measure.removePosition = new Vector3(0, GameManager.Instance.deadZoneYAxis, 0);
        measure.measureCount = measureCount;
        measure.conductor = conductor;
        measure.fretboardScale = fretboardScale;

        // full measure so make it different colored
        if (measureLine)
        {
            measure.GetComponent<LineRenderer>().material = fullMeasureLineMaterial;
        }

    }

    /// <summary>
    /// Find the vertices for where the fretboard vertical lines should start
    /// </summary>
    void FindTopInbetweenLocation()
    {
        topInbetweenLocations = new Vector3[inbetweenFingerBoard.Length];

        for (int i = 0; i < inbetweenFingerBoard.Length; ++i)
        {
            topInbetweenLocations[i] = new Vector3(inbetweenFingerBoard[i].x * spawnWidthPercent, spawnHeight, 0);
        }
    }

    /// <summary>
    /// Find the vertices for where the fretboard vertical lines should pass in between the fingerbuttons
    /// </summary>
    void FindInbetweenLocations()
    {
        // get the distance between any two points
        float singleDistanceX = 0;
        if (fingerBoard.positions.Length > 1)
        {
            singleDistanceX = Mathf.Abs(fingerBoard.positions[0].x - fingerBoard.positions[1].x);
        }

        inbetweenFingerBoard = new Vector3[fingerBoard.positions.Length + 1];
        for (int i = 0; i < fingerBoard.positions.Length; ++i)
        {
            inbetweenFingerBoard[i] = new Vector3(
                fingerBoard.positions[i].x - (singleDistanceX/2),
                fingerBoard.positions[i].y,
                fingerBoard.positions[i].z
                );
        }
        // set the last one
        inbetweenFingerBoard[^1] = new Vector3(
            fingerBoard.positions[^1].x + (singleDistanceX / 2),
            fingerBoard.positions[^1].y,
            fingerBoard.positions[^1].z
            );
    }

    /// <summary>
    /// Calculater the vertices for where the vertical lines on the fretboard should end
    /// </summary>
    private void CalculateRemoveXPositions()
    {
        for (int i = 0; i < inbetweenFingerBoard.Length; ++i)
        {
            // Uses radians
            float topAngle = Mathf.Atan(Mathf.Abs(inbetweenFingerBoard[i].x - topInbetweenLocations[i].x) / Mathf.Abs(transform.position.y - inbetweenFingerBoard[i].y));
            float removeX = noteVerticalTravelDistance * Mathf.Tan(topAngle);

            // Moving left to right set negative since at middle
            if (i < inbetweenFingerBoard.Length / 2)
            {
                inbetweenFingerBoard[i].x = -removeX + topInbetweenLocations[i].x;
            }
            else
            {
                inbetweenFingerBoard[i].x = removeX + topInbetweenLocations[i].x;
            }
            inbetweenFingerBoard[i].y -= 2;
        }
    }

    /// <summary>
    /// helper function for drawing lines
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPosition"></param>
    /// <param name="color"></param>
    /// <param name="line"></param>
    void DrawLine(Vector3 startPosition, Vector3 endPosition, Color color, GameObject line)
    {
        line.transform.parent = transform;
        line.transform.position = startPosition;
        line.AddComponent<LineRenderer>();

        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(lineMaterial);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.sortingOrder = 1;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    /// <summary>
    /// function for drawing fret background
    /// </summary>
    void DrawFretBackground()
    {
        GameObject fretBackground = new GameObject();
        fretMesh = new Mesh();
        fretMeshFilter = fretBackground.AddComponent<MeshFilter>();
        meshRenderer = fretBackground.AddComponent<MeshRenderer>();
        meshRenderer.material = fretBoardMaterial;

        Vector3[] vertices = new Vector3[4]
        { 
            topInbetweenLocations[0],
            topInbetweenLocations[^1],
            inbetweenFingerBoard[^1],
            inbetweenFingerBoard[0]
        };
        int[] triangles = new int[6]{ 0, 1, 2, 0, 2, 3 };
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        fretBackground.GetComponent<Transform>().parent = transform;
        fretMesh.vertices = vertices;
        fretMesh.triangles = triangles;
        fretMesh.normals = normals;

        fretMeshFilter.mesh = fretMesh;
    }
}
