using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the camera moves
    public float zoomSpeed = 5f;  // Speed at which the camera zooms in/out
    public float minZoom = 2f;    // Minimum zoom level
    public float maxZoom = 10f;   // Maximum zoom level

    // Boundaries for camera movement
    public float minX = -10f;     // Minimum X position
    public float maxX = 10f;      // Maximum X position
    public float minY = -10f;     // Minimum Y position
    public float maxY = 10f;      // Maximum Y position

    private Vector3 moveDirection;

    void Update()
    {
        HandleCameraMovement();
        HandleZoom();
    }

    void HandleCameraMovement()
    {
        // Reset the moveDirection to zero
        moveDirection = Vector3.zero;

        // Check for input and set the move direction
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.up;  // Move up (positive Y direction)
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.down;  // Move down (negative Y direction)
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;  // Move left (negative X direction)
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;  // Move right (positive X direction)
        }

        // Apply the movement to the camera position
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Clamp the camera's position to stay within the defined boundaries
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void HandleZoom()
    {
        // Zoom in/out with the mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);  // Limit zoom range
        }
    }
}
