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
                Transform clickedPiece = hit.transform;
                RotateGroup(clickedPiece, 90);
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
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
    }

    private void RotateGroup(Transform clickedPiece, float angle)
    {
        List<UnityEngine.Object> groupToRotate = null;

        // Find the group that contains the clicked piece
        foreach (var group in pieceGroups)
        {
            if (group.Contains(clickedPiece))
            {
                groupToRotate = group;
                break;
            }
        }

        if (groupToRotate == null) return;

        Vector3 pivot = clickedPiece.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Rotate each piece in the group around the clicked piece
        foreach (var groupPiece in groupToRotate)
        {
            if (groupPiece is Transform pieceTransform)
            {
                Vector3 direction = pieceTransform.position - pivot;
                direction = rotation * direction;
                pieceTransform.position = pivot + direction;
                pieceTransform.Rotate(Vector3.forward, angle);
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
        List<UnityEngine.Object> pieceGroup = null;

        // Find the group that contains the piece
        foreach (var group in pieceGroups)
        {
            if (group.Contains(piece))
            {
                pieceGroup = group;
                break;
            }
        }

        foreach (var group in pieceGroups)
        {
            foreach (var groupPiece in group)
            {
                if (groupPiece is Transform groupPieceTransform)
                {
                    string side = GetTouchingSide(piece, groupPieceTransform, pieceWidth, pieceHeight, tolerance);
                    if (side != "None")
                    {
                        // if not part of the same group, align
                        if (pieceGroup != null && !pieceGroup.Contains(groupPieceTransform))
                        {
                            AlignPiece(piece, groupPieceTransform, side, pieceGroup);
                        }
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

    private void AlignPiece(Transform piece, Transform otherPiece, string side, List<UnityEngine.Object> group)
    {
        //Debug.Log("Aligning pieces");
        float pieceWidth = piece.GetComponent<Renderer>().bounds.size.x;
        float pieceHeight = piece.GetComponent<Renderer>().bounds.size.y;
        Vector3 pos1 = piece.position;
        Vector3 pos2 = otherPiece.position;
        Vector3 offset = Vector3.zero;

        switch (side)
        {
            case "Left":
                offset = new Vector3(pos2.x + pieceWidth - pos1.x, pos2.y - pos1.y, 0);
                piece.position = new Vector3(pos2.x + pieceWidth, pos2.y, pos1.z);
                break;
            case "Right":
                offset = new Vector3(pos2.x - pieceWidth - pos1.x, pos2.y - pos1.y, 0);
                piece.position = new Vector3(pos2.x - pieceWidth, pos2.y, pos1.z);
                break;
            case "Top":
                offset = new Vector3(pos2.x - pos1.x, pos2.y - pieceHeight - pos1.y, 0);
                piece.position = new Vector3(pos2.x, pos2.y - pieceHeight, pos1.z);
                break;
            case "Bottom":
                offset = new Vector3(pos2.x - pos1.x, pos2.y + pieceHeight - pos1.y, 0);
                piece.position = new Vector3(pos2.x, pos2.y + pieceHeight, pos1.z);
                break;
        }

        // Apply the same offset to all pieces in the group
        foreach (var groupPiece in group)
        {
            if (groupPiece is Transform groupPieceTransform && groupPieceTransform != piece)
            {
                groupPieceTransform.position += offset;
            }
        }
    }

}
