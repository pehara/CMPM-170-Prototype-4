using System;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    private Vector3 offset;
    private float rotation;
    private bool isDragging = false;

    void Start()
    {

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

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit && hit.transform == transform)
            {
                rotation = transform.eulerAngles.z;
                transform.eulerAngles = new Vector3(0, 0, rotation + 90);
            }
        }
    }
}
