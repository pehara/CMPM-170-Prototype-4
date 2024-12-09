using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtGenerator : MonoBehaviour
{
    [SerializeField] private List<float> transparencies;
    [SerializeField] private Transform dirtPrefab;
    private Vector2 dimensions;
    private float orthoHeight;
    private float orthoWidth;
    private float screenAspect;
    public bool digEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transparencies = new List<float>() {1.0f, 0.75f, 0.5f, 0.25f};
        dimensions = GetDimensionsofScreen(5);
        GenerateDirt();
    }

    // Update is called once per frame
    void Update()
    {
        if (digEnabled) {
            if (Input.GetMouseButtonDown(0))
            {
                LayerMask layermask = LayerMask.GetMask("Dirt");
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layermask);
                if (hit && hit.transform.tag == "Dirt")
                {
                    GameObject dirt = hit.transform.gameObject;
                    SpriteRenderer renderer = dirt.GetComponent<SpriteRenderer>();
                    float newAlpha = Mathf.Max(0, renderer.color.a - 0.25f); // Ensure alpha doesn't go below 0
                    renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, newAlpha);
                    if (renderer.color.a <= 0) {
                        Destroy(dirt);
                    }
                }
            }
        }
        
    }

    public void enableDigMode() 
    {
        digEnabled = true;
    }

    public void disableDigMode()
    {
        digEnabled = false;
    }

    private Vector2 GetDimensionsofScreen(float degree) 
    {
        Vector2 dimensions = Vector2.zero;
        orthoHeight = Camera.main.orthographicSize;
        screenAspect = (float)Screen.width / Screen.height;
        orthoWidth = screenAspect * orthoHeight;

        if (orthoWidth < orthoHeight) {
            dimensions.x = degree;
            dimensions.y = (degree * orthoHeight) / orthoWidth;
        } else {
            dimensions.x = (degree * orthoWidth) / orthoHeight;
            dimensions.y = degree;
        }
        return dimensions;
    }

    private void GenerateDirt() 
    {
        float height = orthoHeight/dimensions.y;
        float width = orthoWidth/dimensions.x;
        float scale = 2f;
        height *= scale;
        width *= scale;

        for (int row = 0; row < dimensions.y; row++) {
            for (int col = 0; col < dimensions.x; col++) {
                Transform dirt = Instantiate(dirtPrefab);
                dirt.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    -1);
                dirt.localScale = new Vector3(width, height, 1f);
                float transparencyLevel = transparencies[Random.Range(0, transparencies.Count - 1)];
                SpriteRenderer renderer = dirt.GetComponent<SpriteRenderer>();
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, transparencyLevel);

                // name each piece programmatically to show up in the Scene View
                dirt.name = $"Dirt {(row * dimensions.x) + col}";
            }
        }
    }

}
