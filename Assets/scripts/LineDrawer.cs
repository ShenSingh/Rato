using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // අලුත් Input System එක සඳහා මෙය අවශ්‍යයි

public class LineDrawer : MonoBehaviour
{
    public GameObject linePrefab;

    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private List<Vector2> fingerPositions;

    void Update()
    {
        // පරිශීලකයා තිරය මත touch කළ විට (අලුත් ක්‍රමය)
        if (Pointer.current.press.wasPressedThisFrame)
        {
            CreateLine();
        }

        // පරිශීලකයා තිරය මත ඇඟිල්ල ගෙන යන විට (අලුත් ක්‍රමය)
        if (Pointer.current.press.isPressed)
        {
            // currentLine එකක් ඇත්නම් පමණක් update කරන්න
            if (currentLine != null)
            {
                Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
                if (Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > .1f)
                {
                    UpdateLine(tempFingerPos);
                }
            }
        }
    }

    void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        fingerPositions = new List<Vector2>();

        Vector2 startPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        fingerPositions.Add(startPos);
        fingerPositions.Add(startPos);
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);
        edgeCollider.points = fingerPositions.ToArray();
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
        edgeCollider.points = fingerPositions.ToArray();
    }
}