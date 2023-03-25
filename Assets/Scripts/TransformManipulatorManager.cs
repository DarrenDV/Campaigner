using System;
using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Serialization;

public class TransformManipulatorManager : MonoBehaviour
{   
    [SerializeField] private RuntimeTransformHandle _transformHandle;

    private Camera _mainCamera;
    
    private void Start()
    {
        if (_transformHandle == null)
        {
            _transformHandle = GameObject.FindGameObjectWithTag("TransformHandler").GetComponent<RuntimeTransformHandle>();
        }

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;

        }
    }

    private void Update()
    {
        if (!_transformHandle.gameObject.activeSelf) //If the transform handle is not active we can select whatever
        {
            CheckForSelectedObject();
        }
        else if(_transformHandle.gameObject.activeSelf && !MouseHoveringOverHandle()) //If the transform handle is active and the mouse is not hovering over a handle
        {
            CheckForSelectedObject();
        }
        
        TransformInput();
    }

    private void TransformInput()
    {
        if (_transformHandle.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _transformHandle.type = HandleType.POSITION;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                _transformHandle.type = HandleType.ROTATION;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                _transformHandle.type = HandleType.SCALE;
            }
            
            //Basically if the user presses escape
            if(Input.GetKeyDown(KeyCode.X))
            {
                DisableTransformHandler();
            }
        }
    }
    
    /// <summary>
    /// Checks if the user has clicked on a selectable object and if so enables the transform handle
    /// </summary>
    private void CheckForSelectedObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Selectable"))
                {
                    _transformHandle.gameObject.SetActive(true);
                    _transformHandle.target = hit.transform;
                }
            }
            else
            {
                DisableTransformHandler();
            }
        }
    }

    /// <summary>
    /// Disables the transform handle and resets variables
    /// </summary>
    private void DisableTransformHandler()
    {
        _transformHandle.target = null;
        _transformHandle.type = HandleType.POSITION;
        _transformHandle.gameObject.SetActive(false);
    }

    /// <summary>
    /// Checks if the mouse is hovering over a handle, regardless of objects in between
    /// </summary>
    /// <returns> True if hovering over a handle, false if not </returns>
    private bool MouseHoveringOverHandle()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length == 0)
            return false;

        foreach (RaycastHit hit in hits)
        {
            HandleBase handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();
            
            if (handle != null)
            {
                return true;
            }
        }
        
        return false;
    }
}
