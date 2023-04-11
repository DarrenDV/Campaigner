using System;
using System.Collections;
using System.Collections.Generic;
using Campaigner.UI;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Serialization;

public class TransformManipulatorManager : MonoBehaviour
{   
    /*
     *  This class is used to manage the transform handles.
     *  The player can select a gameobject and a transform handle will appear. The player can then move, rotate or scale the object.
     *  The player currently is not able to select multiple objects at once. I might want to change this later on however.
     *  
     *  A bunch of code is semi-hardcoded, this is because I am not sure how I want to implement this yet.
     */
    
    public static TransformManipulatorManager Instance { get; private set; }
    
    public event Action OnTransformHandleEnabled;
    public event Action OnTransformHandleDisabled;
    
    
    [SerializeField] private RuntimeTransformHandle _transformHandle;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Color outlineColor;
    [SerializeField] private float outlineWidth;
    
    private Camera _mainCamera;
    
    

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
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                DisableTransformHandler();
            }
        }
    }
    
    public void SetHandleType(HandleType handleType)
    {
        if (_transformHandle.gameObject.activeSelf)
        {
            _transformHandle.type = handleType;
        }
    }
    
    /// <summary>
    /// Checks if the user has clicked on a selectable object and if so enables the transform handle
    /// </summary>
    private void CheckForSelectedObject()
    {
        if (UIUtils.Instance.IsPointerOverUIElement())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Selectable"))
                {
                    if (_transformHandle.gameObject.activeSelf)
                    {
                        DisableTransformHandler();
                    }
                    SetSelectedObject(hit);
                }
            }
            else
            {
                DisableTransformHandler();
            }
        }
    }

    private void SetSelectedObject(RaycastHit hit)
    {
        OnTransformHandleEnabled?.Invoke();
        _transformHandle.gameObject.SetActive(true);
        _transformHandle.target = hit.transform;
        
        
        Renderer renderer = hit.transform.gameObject.GetComponent<Renderer>();
        Material[] materials = new Material[renderer.materials.Length + 1];
        renderer.materials.CopyTo(materials, 0);
        materials[materials.Length - 1] = selectedMaterial;
        renderer.materials = materials;
        
      
                    
        GameUIManager.Instance.CanSwitchMenuState = false;
    }

    /// <summary>
    /// Disables the transform handle and resets variables
    /// </summary>
    private void DisableTransformHandler()
    {
        if (_transformHandle.target == null)
        {
            return;
        }
        
        Renderer renderer = _transformHandle.target.gameObject.GetComponent<Renderer>();
        Material[] materials = new Material[renderer.materials.Length - 1];
        Array.Copy(renderer.materials, materials, materials.Length);
        renderer.materials = materials;
        
        
        OnTransformHandleDisabled?.Invoke();
        _transformHandle.target = null;
        _transformHandle.type = HandleType.POSITION;
        _transformHandle.gameObject.SetActive(false);

        GameUIManager.Instance.CanSwitchMenuState = true;
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
