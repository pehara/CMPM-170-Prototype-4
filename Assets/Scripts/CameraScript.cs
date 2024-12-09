using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Minimum and maximum orthographic size
    public float minOrthographicSize = 2.0f;
    public float maxOrthographicSize = 15.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // zoom out or in when user scrolls
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && IsMouseInGameWindow())
        {
            Camera.main.orthographicSize -= scroll * 2;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minOrthographicSize, maxOrthographicSize);
        }

        bool IsMouseInGameWindow()
        {
            return Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width &&
                   Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height;
        }
    }
}
