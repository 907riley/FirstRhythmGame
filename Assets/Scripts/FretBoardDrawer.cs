using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FretBoardDrawer : MonoBehaviour
{
    [SerializeField] Material lineMaterial;
    [SerializeField] Material fretBoardMaterial;
    [SerializeField] GameObject fingerBoardGo;
    private FingerBoard fingerBoard;

    [SerializeField] GameObject gameManagerGo;
    private GameManager gameManager;
    // the dots inbetween the fingerButtons where the lines should go for lanes
    private Vector3[] inbetweenFingerBoard;
    // how far the lines go vertically, for triangles
    private float noteVerticalTravelDistance;

    private Vector3[] topInbetweenLocations;

    private float spawnWidthPercent;
    private float spawnHeight;

    private Mesh fretMesh;
    private MeshFilter fretMeshFilter;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        fingerBoard = fingerBoardGo.GetComponent<FingerBoard>();
        //gameManager = gameManagerGo.GetComponent<GameManager>();

        transform.position = new Vector3(0, GameManager.Instance.spawnHeight, 0);

        noteVerticalTravelDistance = Mathf.Abs(transform.position.y - fingerBoard.positions[0].y) + 2;

        //spawnWidthPercent = gameManager.spawnWidthPercent;
        //spawnHeight = gameManager.spawnHeight;

        spawnWidthPercent = GameManager.Instance.spawnWidthPercent;
        spawnHeight = GameManager.Instance.spawnHeight;

        // the points where the fretboard vertical lines should intersect the fingerboard
        FindInbetweenLocations();
        // the points where the fretboard verticals should start at the top of the fretboard
        FindTopInbetweenLocation();
        // the points where the fretboard vertical lines should end off screen
        CalculateRemoveXPositions();

        //Debug print out the inbetweens
        //foreach (Vector3 locall in inbetweenFingerBoard)
        //{
        //    Debug.Log(locall);

        //}
        DrawFretBackground();
        for (int i = 0; i < inbetweenFingerBoard.Length; ++i)
        {
            DrawLine(topInbetweenLocations[i], inbetweenFingerBoard[i], new Color(0.6f, 0.7f, 0.2f, 1));
        }

        DrawLine(topInbetweenLocations[0], topInbetweenLocations[^1], new Color(0, 0, 0, 1));
        
    }

    void FindTopInbetweenLocation()
    {
        topInbetweenLocations = new Vector3[inbetweenFingerBoard.Length];

        for (int i = 0; i < inbetweenFingerBoard.Length; ++i)
        {
            topInbetweenLocations[i] = new Vector3(inbetweenFingerBoard[i].x * spawnWidthPercent, spawnHeight, 0);
        }
    }

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

    void DrawLine(Vector3 startPosition, Vector3 endPosition, Color color)
    {
        GameObject line = new GameObject();
        line.transform.parent = transform;
        line.transform.position = startPosition;
        line.AddComponent<LineRenderer>();

        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(lineMaterial);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        //lineRenderer.colorGradient = 1;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        //lineRenderer.widthCurve = 
        lineRenderer.sortingOrder = 1;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

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
