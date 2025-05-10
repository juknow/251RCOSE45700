using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenSetup : MonoBehaviour
{
    public Camera player1Camera;
    public Camera player2Camera;
    public Camera mainCamera;

    void Start()
    {
        SetupCameras();
    }

    void SetupCameras()
    {
        // Player - 1 Camera : Left
        player1Camera.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);

        // Player - 2 Camera : Right
        player2Camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

        // Main Camera : 
        mainCamera.rect = new Rect(0f, 0f, 1f, 0.5f);

        // Depth setting
        player1Camera.depth = 0;
        player2Camera.depth = 0;
        mainCamera.depth = 1;

    }
}
