using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// ACKNOWLEDGEMENTS: 
// Followed tutorial here: https://www.youtube.com/watch?v=OFC_UUaS4gs

public class PieceGenerator : MonoBehaviour
{
    [SerializeField] private int difficulty = 4;
    [SerializeField] private Texture2D puzzleTexture;
    [SerializeField] private Transform puzzlePieceHolder;
    [SerializeField] private Object pieceManager;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // instantiate a list of pieces to track them later
        pieces = new List<Transform>();

        // calculate size of each piece based on dimensions
        dimensions = GetDimensions(puzzleTexture, difficulty);

        // split up image into movable pieces
        CreateJigsawPieces(puzzleTexture);

        // scatter pieces randomly across visible screen
        ScatterPieces();


        // Initialize pieces in PieceScript
        pieceManager.GetComponent<PieceScript>().InitializePieces(pieces);
    }

// Helper Functions --------------
    // Determines number of pieces per given texture
    Vector2Int GetDimensions(Texture2D puzzleTexture, int difficulty) {
        Vector2Int dimensions = Vector2Int.zero;

        // difficulty is the number of pieces cut across either the x or y axis, depending on which is smaller.
        // helps ensure the pieces are as square as possible.
        if (puzzleTexture.width < puzzleTexture.height) {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * puzzleTexture.height) / puzzleTexture.width;
        } else {
            dimensions.x = (difficulty * puzzleTexture.width) / puzzleTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;
    }

    // Create all the jigsaw pieces
    void CreateJigsawPieces(Texture2D puzzleTexture) {
        // calculate piece sizes based on the dimensions
        height = 1f / dimensions.y;
        float aspect = (float)puzzleTexture.width / puzzleTexture.height;
        width = aspect / dimensions.x;

        // iterate over rows and columns
        for (int row = 0; row < dimensions.y; row++) {
            for (int col = 0; col < dimensions.x; col++) {
                
                // create the piece in the right location of the right size.
                Transform piece = Instantiate(piecePrefab, puzzlePieceHolder);
                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    -1);
                piece.localScale = new Vector3(width, height, 1f);

                // name each piece programmatically to show up in the Scene View
                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                // assign the correct part of the texture for this jigsaw piece
                // normalize both width and height between 0 and 1 for the UV.
                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;

                // UV coord order is anti-clockwise: (0, 0), (1, 0), (0, 1), (1, 1)
                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));
                // assign our new UVs to the mesh.
                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;
                // update the texture on the piece
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", puzzleTexture);
            }
        }   
    }

    private void ScatterPieces() {
        // calculate the visible orthographic size of the screen.
        float orthoHeight = Camera.main.orthographicSize;
        float screenAspect = (float)Screen.width / Screen.height;
        float orthoWidth = (screenAspect * orthoHeight);

        // ensure pieces are away from the edges
        float pieceWidth = width * puzzlePieceHolder.localScale.x;
        float pieceHeight = height * puzzlePieceHolder.localScale.y;

        // adjust dimensions of camera view to exclude a border that ensures pieces will be in view
        orthoHeight -= pieceHeight;
        orthoWidth -= pieceWidth;

        // place each piece randomly in the visible area, and randomize the rotation
        foreach (Transform piece in pieces) {
            float x = Random.Range(-orthoWidth, orthoWidth);
            float y = Random.Range(-orthoHeight, orthoHeight);
            piece.position = new Vector3(x, y, -1);

            List<int> possibleAngles = new List<int>() {0, 90, -180, -90};
            int z = possibleAngles[Random.Range(0, possibleAngles.Count - 1)];
            Debug.Log(z);
            piece.eulerAngles = new Vector3(0, 0, z);
        }
    }
}
