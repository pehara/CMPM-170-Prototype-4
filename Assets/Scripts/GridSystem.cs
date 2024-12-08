using UnityEngine;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    public int width = 10; // Number of columns
    public int height = 5; // Number of rows
    public float cellSize = 1f; // Size of each cell in the grid

    private Vector3 initialPosition; // Store initial grid position
    private HashSet<Vector2Int> lockedPositions = new HashSet<Vector2Int>(); // Tracks locked positions

    private void Awake()
    {
        // Store  initial position at the start
        initialPosition = transform.position;
    }

    private void Start()
    {
        // Reset grid to its initial state at runtime
        ResetGridToInitialState();
    }

    private void ResetGridToInitialState()
    {
        transform.position = initialPosition; // Makes grid stay in its original position
        Debug.Log($"Grid reset: Position={initialPosition}, Width={width}, Height={height}, Cell Size={cellSize}");
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize) + transform.position;
    }

    public Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - transform.position.y) / cellSize);
        return new Vector3Int(x, y, 0);
    }

    public void LockPiece(Vector2Int position)
    {
        lockedPositions.Add(position);
        Debug.Log($"Position {position} locked.");
    }

    public bool IsPositionLocked(Vector2Int position)
    {
        return lockedPositions.Contains(position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int x = 0; x <= width; x++)
        {
            Vector3 startPoint = GetWorldPosition(x, 0);
            Vector3 endPoint = GetWorldPosition(x, height);
            Gizmos.DrawLine(startPoint, endPoint);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 startPoint = GetWorldPosition(0, y);
            Vector3 endPoint = GetWorldPosition(width, y);
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
