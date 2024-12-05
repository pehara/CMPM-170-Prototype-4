using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    //public Material[] rockMaterials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        collider.pathCount = 0; // Reset the collider
        List<Vector2> physicsShape = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
        collider.SetPath(0, physicsShape.ToArray());
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
            }
        }
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            transform.position += Vector3.forward * 0.1f;
            isDragging = false;
        }
        if (isDragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition += offset;
            transform.position = newPosition;
        }

    }
}
