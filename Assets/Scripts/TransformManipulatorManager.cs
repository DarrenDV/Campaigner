using System;
using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Serialization;

public class TransformManipulatorManager : MonoBehaviour
{   
    [SerializeField] private RuntimeTransformHandle _transformHandle;
    
    private void Start()
    {
        if (_transformHandle == null)
        {
            _transformHandle = GameObject.FindGameObjectWithTag("TransformHandler").GetComponent<RuntimeTransformHandle>();
        }
    }

    private void Update()
    {
        if (!_transformHandle.gameObject.activeSelf)
        {
            CheckForSelectedObject();
            Debug.Log("Dont need to check hovering shit");
        }
        else if(_transformHandle.gameObject.activeSelf && !MouseHoveringOverHandle())
        {
            Debug.Log("need to check hovering shit");
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
    
    private void CheckForSelectedObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

    private void DisableTransformHandler()
    {
        _transformHandle.target = null;
        _transformHandle.type = HandleType.POSITION;
        _transformHandle.gameObject.SetActive(false);
    }

    private bool MouseHoveringOverHandle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.GetComponentInParent<HandleBase>())
            {
                Debug.Log("shit is hovering over");
                return true;
            }
        }
        
        return false;
    }
}
