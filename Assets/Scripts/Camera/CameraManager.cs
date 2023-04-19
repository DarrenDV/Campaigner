using System;
using System.Collections;
using System.Collections.Generic;
using Campaigner.UI;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    private CameraController _cameraController;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _cameraController = GetComponent<CameraController>();
        GameUIManager.Instance.OnGamePausedChanged += ToggleCameraController; //Addlistener in start because it nullrefs in OnEnable
    }

    // private void OnEnable()
    // {
    //     Debug.Log(GameUIManager.Instance);
    //     
    // }

    private void ToggleCameraController(bool gamePaused)
    {
        _cameraController.enabled = !gamePaused;
    }

    private void OnDisable()
    {
        GameUIManager.Instance.OnGamePausedChanged -= ToggleCameraController;
    }
}