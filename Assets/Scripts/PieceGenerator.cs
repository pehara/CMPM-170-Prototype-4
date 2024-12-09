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
    [SerializeField] private GridSystem gridSystem;

    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float pieceWidth;
    private float pieceHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // instantiate a list of pieces to track them later
        pieces = new List<Transform>();

        // calculate size of each piece based on dimensions
        dimensions = GetDimensions(puzzleTexture, difficulty);

        pieceHeight = 1f / dimensions.y;
        float aspect = (float)puzzleTexture.width / puzzleTexture.height;
        pieceWidth = aspect / dimensions.x;

        Debug.Log($"Piece width: {pieceWidth}, Piece height: {pieceHeight}");
        Debug.Log($"Grid dimensions: {dimensions.x}x{dimensions.y}");

        // Pieces are generated relative to the fixed grid
        CreateJigsawPieces(puzzleTexture);
        ScatterPieces();


        // Initialize pieces in PieceScript
        pieceManager.GetComponent<PieceScript>().InitializePieces(pieces);
    }

    Vector2Int GetDimensions(Texture2D texture, int diff)
    {
        Vector2Int dims = Vector2Int.zero;
        if (texture.width < texture.height)
        {
            dims.x = diff;
            dims.y = (diff * texture.height) / texture.width;
        }
        else
        {
            dims.x = (diff * texture.width) / texture.height;
            dims.y = diff;
        }
        return dims;
    }

    void CreateJigsawPieces(Texture2D texture)
    {
        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                Transform piece = Instantiate(piecePrefab, puzzlePieceHolder);
                piece.position = gridSystem.GetWorldPosition(col, row); // Align pieces with grid
                piece.localScale = new Vector3(pieceWidth, pieceHeight, 1f);
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
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);

                piece.GetComponent<PieceScript>().Initialize(gridSystem, new Vector2Int(col, row));
            }
        }
    }

    private void ScatterPieces() {
        // calculate the visible orthographic size of the screen.
        float orthoHeight = Camera.main.orthographicSize;
        float screenAspect = (float)Screen.width / Screen.height;
        float orthoWidth = screenAspect * orthoHeight;

        foreach (Transform piece in pieces)
        {
            float x = Random.Range(-orthoWidth, orthoWidth);
            float y = Random.Range(-orthoHeight, orthoHeight);
            piece.position = new Vector3(x, y, piece.position.z); // Maintain Z-axis
            List<int> possibleAngles = new List<int>() { 0, 90, 180, 270 };
            int z = possibleAngles[Random.Range(0, possibleAngles.Count)];
            piece.eulerAngles = new Vector3(0, 0, z);
        }
    }
}
