using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager Instance { get; private set; }

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

    private void Update()
    {
        HandleBuildMode();
    }

    private void HandleBuildMode()
    {
        if (UIUtils.Instance.IsPointerOverUIElement())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }
}
