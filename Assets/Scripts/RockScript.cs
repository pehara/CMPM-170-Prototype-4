using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockScript : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    public GameObject trashIcon; // Reference to the trash icon game object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        collider.pathCount = 0; // Reset the collider
        List<Vector2> physicsShape = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
        collider.SetPath(0, physicsShape.ToArray());

        // Ensure the trash icon is initially hidden
        if (trashIcon != null)
        {
            trashIcon.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit && hit.transform == transform)
            {
                isDragging = true;
                offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset += Vector3.back * 0.1f;

                // Show the trash icon when dragging starts
                if (trashIcon != null)
                {
                    trashIcon.SetActive(true);
                }
            }
        }
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            transform.position += Vector3.forward * 0.1f;
            isDragging = false;

            // Check if the rock is dropped in top right corner
            var viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x > 0.85 && viewportPos.y > 0.85)
            {
                Destroy(gameObject);
            }
            // Hide the trash icon when dragging stops
            trashIcon.SetActive(false);
        }
        if (isDragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition += offset;
            transform.position = newPosition;
        }
    }
}
