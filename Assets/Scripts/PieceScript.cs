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
    public bool piecesEnabled = true;

    void Start()
    {
        // Just in case pieces go too far from camera
        initialZ = -1.117f;
    }

    public void InitializePieces(List<Transform> pieces)
    {
        // Ensure each piece is part of a group
        foreach (Transform piece in pieces)
        {
            if (!IsPieceInAnyGroup(piece))
            {
                pieceGroups.Add(new List<UnityEngine.Object> { piece });
            }
        }
    }

    void Update()
    {
        if (piecesEnabled) {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit && hit.transform.tag != "Rock" && hit.transform.tag != "Dirt")
                {
                    draggingPiece = hit.transform;
                    offset = draggingPiece.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    offset += Vector3.back;

                    // Calculate initial offsets for the group when dragging one piece
                    initialGroupOffsets.Clear();
                    var group = GetGroupContainingPiece(draggingPiece);
                    if (group != null)
                    {
                        foreach (var piece in group)
                        {
                            if (piece is Transform pieceTransform)
                            {
                                initialGroupOffsets.Add(pieceTransform.position - new Vector3(draggingPiece.position.x, draggingPiece.position.y, 0));
                                pieceTransform.position += Vector3.back;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Dragging piece not found in any group");
                    }
                }
            }

            if (draggingPiece && Input.GetMouseButtonUp(0))
            {
                draggingPiece.position = new Vector3(draggingPiece.position.x, draggingPiece.position.y, initialZ);
                var group = GetGroupContainingPiece(draggingPiece);
                if (group != null)
                {
                    foreach (var piece in group)
                    {
                        if (piece is Transform pieceTransform && pieceTransform != draggingPiece)
                        {
                            pieceTransform.position = new Vector3(pieceTransform.position.x, pieceTransform.position.y, initialZ);
                        }
                    }
                }
                checkPiecePlacement(draggingPiece);
                draggingPiece = null;
            }

            if (draggingPiece)
            {
                DragPiece();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit && hit.transform.tag != "Rock" && hit.transform.tag != "Dirt")
                {
                    if (IsPieceInGroupWithMoreThanOne(hit.transform))
                    {
                        return;
                    }
                    rotation = hit.transform.eulerAngles.z;
                    hit.transform.eulerAngles = new Vector3(0, 0, NormalizeAngle(rotation + 90));
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit && hit.transform.tag != "Rock" && hit.transform.tag != "Dirt")
                {
                    if (IsPieceInGroupWithMoreThanOne(hit.transform))
                    {
                        return;
                    }
                    rotation = hit.transform.eulerAngles.z;
                    hit.transform.eulerAngles = new Vector3(0, 0, NormalizeAngle(rotation - 90));
                }
            }

            if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Space))
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
    }

    public void enablePieceMode() 
    {
        piecesEnabled = true;
        
    }

    public void disablePieceMode() 
    {
        piecesEnabled = false;
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    private bool IsPieceInAnyGroup(Transform piece)
    {
        foreach (var group in pieceGroups)
        {
            if (group.Contains(piece))
            {
                return true;
            }
        }
        return false;
    }

    private List<UnityEngine.Object> GetGroupContainingPiece(Transform piece)
    {
        foreach (var group in pieceGroups)
        {
            if (group.Contains(piece))
            {
                return group;
            }
        }
        return null;
    }

    private bool IsPieceInGroupWithMoreThanOne(Transform piece)
    {
        var group = GetGroupContainingPiece(piece);
        return group != null && group.Count > 1;
    }

    private void DragPiece()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition += offset;

        // Update positions of all pieces in the group
        var group = GetGroupContainingPiece(draggingPiece);
        if (group != null)
        {
            for (int i = 0; i < group.Count; i++)
            {
                if (group[i] is Transform piece)
                {
                    Vector3 newPiecePosition = newPosition + initialGroupOffsets[i];
                    piece.position = newPiecePosition;
                }
            }
        }
    }
    private string AreCorrectNeighbors(Transform piece1, Transform piece2)
    {
        // Assuming piece names are in the format "Piece X" where X is the piece index
        int index1 = int.Parse(piece1.name.Split(' ')[1]);
        int index2 = int.Parse(piece2.name.Split(' ')[1]);

        int width = 5; // Number of pieces in a row
        //int height = 4; // Number of pieces in a column

        // Calculate row and column for each piece
        int row1 = index1 / width;
        int col1 = index1 % width;
        int row2 = index2 / width;
        int col2 = index2 % width;

        // Check if the rotation is 0
        bool sameRotation = Mathf.Approximately(piece1.eulerAngles.z, piece2.eulerAngles.z) && Mathf.Approximately(piece1.eulerAngles.z, 0);
        if (!sameRotation)
        {
            return "None";
        }

        // check which side the piece is supposed to be touching
        bool isLeft = (col1 == col2 + 1) && (row1 == row2);
        bool isRight = (col1 == col2 - 1) && (row1 == row2);
        bool isTop = (row1 == row2 - 1) && (col1 == col2);
        bool isBottom = (row1 == row2 + 1) && (col1 == col2);

        if (isLeft)
        {
            return "Left";
        }
        else if (isRight)
        {
            return "Right";
        }
        else if (isTop)
        {
            return "Top";
        }
        else if (isBottom)
        {
            return "Bottom";
        }
        return "None";
    }

    private void checkPiecePlacement(Transform piece)
    {
        float pieceWidth = piece.GetComponent<Renderer>().bounds.size.x;
        float pieceHeight = piece.GetComponent<Renderer>().bounds.size.y;
        float tolerance = 0.1f;

        List<UnityEngine.Object> currentGroup = null;
        List<UnityEngine.Object> pieceGroup = GetGroupContainingPiece(piece);
        bool foundGroupToMerge = false;

        foreach (var group in pieceGroups)
        {
            foreach (var groupPiece in group)
            {
                if (groupPiece is Transform groupPieceTransform)
                {
                    string side = GetTouchingSide(piece, groupPieceTransform, pieceWidth, pieceHeight, tolerance);
                    if (side != "None" && !foundGroupToMerge)
                    {
                        if (side != AreCorrectNeighbors(piece, groupPieceTransform))
                        {
                            continue;
                        }

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
                            foreach (var groupPiece1 in group)
                            {
                                if (!currentGroup.Contains(groupPiece1))
                                {
                                    currentGroup.Add(groupPiece1);
                                }
                            }
                            pieceGroups.Remove(group);
                            foundGroupToMerge = true;
                            break;
                        }
                    }
                }
            }
            // if groupsToMerge is not empty, we have already merged the groups
            if (foundGroupToMerge)
            {
                Debug.Log("Merged groups");
                break;
            }
            if (currentGroup != null && currentGroup != group)
            {
                Debug.Log("Found a group to merge");
                break;
            }
        }

        if (currentGroup == null)
        {
            currentGroup = new List<UnityEngine.Object> { piece };
            pieceGroups.Add(currentGroup);
        }

        // Move all pieces from pieceGroup to currentGroup if they are different
        if (pieceGroup != null && pieceGroup != currentGroup)
        {
            foreach (var groupPiece in pieceGroup)
            {
                if (!currentGroup.Contains(groupPiece))
                {
                    currentGroup.Add(groupPiece);
                }
            }
            pieceGroups.Remove(pieceGroup);
        }

        CleanupGroups();
    }

    private void CleanupGroups()
    {
        HashSet<UnityEngine.Object> seenPieces = new HashSet<UnityEngine.Object>();

        foreach (var group in pieceGroups)
        {
            for (int i = group.Count - 1; i >= 0; i--)
            {
                if (seenPieces.Contains(group[i]))
                {
                    group.RemoveAt(i);
                }
                else
                {
                    seenPieces.Add(group[i]);
                }
            }
        }

        // Remove empty groups
        pieceGroups.RemoveAll(group => group.Count == 0);
    }

    private string GetTouchingSide(Transform piece1, Transform piece2, float pieceWidth, float pieceHeight, float tolerance = 0.1f)
    {
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
                offset = new Vector3((pos2.x - pieceWidth - pos1.x), (pos2.y - pos1.y), 0);
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
