using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    private Vector3 offset;
    private float rotation;
    private Transform draggingPiece = null;
    private List<Vector3> initialGroupOffsets = new List<Vector3>();
    public List<List<UnityEngine.Object>> pieceGroups = new List<List<UnityEngine.Object>>();
    private float initialZ;

    void Start()
    {
        // get first piece and keep track of its z value
        initialZ = -0.117f;
    }

    public void InitializePieces(List<Transform> pieces)
    {
        // Ensure each piece is part of a group
        foreach (Transform piece in pieces)
        {
            bool isInGroup = false;
            foreach (var group in pieceGroups)
            {
                if (group.Contains(piece))
                {
                    isInGroup = true;
                    break;
                }
            }

            if (!isInGroup)
            {
                pieceGroups.Add(new List<UnityEngine.Object> { piece });
            }
        }

        // Print the contents of pieceGroups
        foreach (var group in pieceGroups)
        {
            string groupContents = "Group: ";
            foreach (var piece in group)
            {
                if (piece is Transform pieceTransform)
                {
                    groupContents += pieceTransform.name + " ";
                }
            }
            Debug.Log(groupContents);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                draggingPiece = hit.transform;
                offset = draggingPiece.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset += Vector3.back;

                // Calculate initial offsets for the group
                initialGroupOffsets.Clear();
                bool foundGroup = false;
                foreach (var group in pieceGroups)
                {
                    if (group.Contains(draggingPiece))
                    {
                        foundGroup = true;
                        foreach (var piece in group)
                        {
                            if (piece is Transform pieceTransform)
                            {
                                initialGroupOffsets.Add(pieceTransform.position - new Vector3(draggingPiece.position.x, draggingPiece.position.y, 0));
                            }
                        }
                        break;
                    }
                }
                if (!foundGroup)
                {
                    Debug.LogWarning("Dragging piece not found in any group");
                }
            }
        }

        if (draggingPiece && Input.GetMouseButtonUp(0))
        {
            draggingPiece.position += Vector3.forward;
            draggingPiece.position = new Vector3(draggingPiece.position.x, draggingPiece.position.y, initialZ);
            checkPiecePlacement(draggingPiece);
            draggingPiece = null;
        }

        if (draggingPiece)
        {
            DragPiece();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                rotation = hit.transform.eulerAngles.z;
                hit.transform.eulerAngles = new Vector3(0, 0, rotation + 90);
            }
        }
    }

    private void DragPiece()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition += offset;

        //Debug.Log("Dragging piece to new position: " + newPosition);

        // Update positions of all pieces in the group
        foreach (var group in pieceGroups)
        {
            if (group.Contains(draggingPiece))
            {
                for (int i = 0; i < group.Count; i++)
                {
                    if (group[i] is Transform piece)
                    {
                        Vector3 newPiecePosition = newPosition + initialGroupOffsets[i];
                        newPiecePosition.z = initialZ;
                        //Debug.Log($"Updating piece {piece.name} to position {newPiecePosition}");
                        piece.position = newPiecePosition;
                    }
                }
                break;
            }
        }
    }

    private void checkPiecePlacement(Transform piece)
    {
        //Debug.Log("Checking piece placement");
        float pieceWidth = piece.GetComponent<Renderer>().bounds.size.x;
        float pieceHeight = piece.GetComponent<Renderer>().bounds.size.y;
        float tolerance = 0.1f; // Adjust this value as needed

        List<UnityEngine.Object> currentGroup = null;
        List<List<UnityEngine.Object>> groupsToMerge = new List<List<UnityEngine.Object>>();

        foreach (var group in pieceGroups)
        {
            foreach (var groupPiece in group)
            {
                if (groupPiece is Transform groupPieceTransform)
                {
                    string side = GetTouchingSide(piece, groupPieceTransform, pieceWidth, pieceHeight, tolerance);
                    if (side != "None")
                    {
                        if (currentGroup == null)
                        {
                            currentGroup = group;
                        }
                        else if (currentGroup != group)
                        {
                            groupsToMerge.Add(group);
                        }

                    }
                }
            }
        }

        if (currentGroup == null)
        {
            currentGroup = new List<UnityEngine.Object> { piece };
            pieceGroups.Add(currentGroup);
        }

        foreach (var group in groupsToMerge)
        {
            currentGroup.AddRange(group);
            pieceGroups.Remove(group);
        }

    }

    private string GetTouchingSide(Transform piece1, Transform piece2, float pieceWidth, float pieceHeight, float tolerance = 0.1f)
    {
        //Debug.Log("Checking touching side");
        Vector3 pos1 = piece1.position;
        Vector3 pos2 = piece2.position;

        float deltaX = pos2.x - pos1.x;
        float deltaY = pos2.y - pos1.y;

        if (Mathf.Abs(deltaX) <= pieceWidth + tolerance && Mathf.Abs(deltaY) < pieceHeight / 2 + tolerance)
        {
            if (deltaX > 0)
            {
                return "Right";
            }
            else
            {
                return "Left";
            }
        }
        else if (Mathf.Abs(deltaY) <= pieceHeight + tolerance && Mathf.Abs(deltaX) < pieceWidth / 2 + tolerance)
        {
            if (deltaY > 0)
            {
                return "Top";
            }
            else
            {
                return "Bottom";
            }
        }

        return "None";
    }



}
