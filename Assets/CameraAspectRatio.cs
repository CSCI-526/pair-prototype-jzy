using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    public float targetAspect = 16f / 9f;  // Adjust this to your desired aspect ratio (e.g., 16:9)

    void Start()
    {
        // Get the current screen aspect ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        // Adjust the camera's viewport rect to match the target aspect ratio
        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            // Add black bars on the top and bottom
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            // Add black bars on the sides
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }
    }
}
