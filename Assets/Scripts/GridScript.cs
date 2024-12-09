using System;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    private Vector3 offset;
    private float rotation;
    private bool isDragging = false;
    
    private GridSystem gridSystem;
    private Vector2Int correctPosition; // Correct grid position for this piece
    private bool isLocked = false; // Track if the piece is locked in place

    // Initialize the piece with a reference to the grid and its correct position
    public void Initialize(GridSystem gridSystem, Vector2Int correctPosition)
    {
        this.gridSystem = gridSystem;
        this.correctPosition = correctPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit && hit.transform == transform && !isLocked)
            {
                isDragging = true;
                offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset += Vector3.back * 0.1f;
            }
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SnapToGrid(); // Snap to the grid when released
        }

        if (isDragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition += offset;
            transform.position = newPosition;
        }

        // Handle rotation
        if (Input.GetMouseButtonDown(1) && !isLocked)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit && hit.transform == transform)
            {
                rotation = transform.eulerAngles.z;
                transform.eulerAngles = new Vector3(0, 0, rotation + 90);
            }
        }
    }

    private void SnapToGrid()
    {
        Vector3Int gridPosition = gridSystem.GetGridPosition(transform.position);
        Vector3 snappedPosition = gridSystem.GetWorldPosition(gridPosition.x, gridPosition.y);

        // Maintain consistent Z-axis during snapping
        snappedPosition.z = transform.position.z;

        Debug.Log($"Snapping {name} to {snappedPosition}");

        transform.position = snappedPosition;

        if (gridPosition == new Vector3Int(correctPosition.x, correctPosition.y, 0) &&
            Mathf.Approximately(transform.eulerAngles.z % 360, 0))
        {
            LockPiece();
            Debug.Log($"Piece {name} placed correctly!");
        }
    }

    private void LockPiece()
    {
        isLocked = true; // Prevent further movement or rotation
        // gridSystem.LockPiece(correctPosition);
    }
}