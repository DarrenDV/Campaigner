using System.Collections.Generic;
using Campaigner.UI;
using UnityEngine;

public class GhostPlacerAndSnapper : MonoBehaviour
{
    [SerializeField] private float snappingRadius = 10f;
    [SerializeField] private int _targetHeight = 0;
    [SerializeField] private GameObject _closestOtherSnapPoint; //this is only serialized for debugging purposes
    [SerializeField] private GameObject _closestOwnSnapPoint; //this is only serialized for debugging purposes

    private GameObject _hookedSnapPoint;
    private GameObject _ghostObject;
    private PlacedObject _ghostObjectScript;
    
    private Camera _mainCam;
    
    public bool hasSnapped = false;
    
    private Quaternion _targetRotation;
    private string previousObjectName = "";
    
    private void Start()
    {
        if (_mainCam == null)
        {
            _mainCam = Camera.main;
        }
    }

    /// <summary>
    /// Function called to set the ghost object, this being the prefab that will be placed
    /// </summary>
    /// <param name="ghostObject"></param>
    public void SetGhostObject(GameObject ghostObject)
    {
        _ghostObject = ghostObject;
        _ghostObjectScript = _ghostObject.GetComponent<PlacedObject>();

        if (_ghostObjectScript.ObjectName == previousObjectName)
        {
            _ghostObject.transform.rotation = _targetRotation;
            _targetRotation = Quaternion.identity;
        }
        
        GameUIManager.Instance.CanSwitchMenuState = false;
    }
    
    /// <summary>
    /// Reset function to clear the ghost object
    /// </summary>
    public void ClearGhostObject()
    {
        previousObjectName = _ghostObjectScript.ObjectName;
        _targetRotation = _ghostObject.transform.rotation;
        
        Destroy(_ghostObject);
        _ghostObjectScript = null;
        _ghostObject = null;
        _hookedSnapPoint = null;
        _closestOtherSnapPoint = null;
        _closestOwnSnapPoint = null;
        hasSnapped = false;
        
        GameUIManager.Instance.CanSwitchMenuState = true;
    }

    private void Update()
    {
        if (_ghostObject == null)
        {
            return;
        }
        
        HandleInput();
        
        Vector3 targetPosition = GetTargetPosition();
        ObjectMovement(targetPosition);
        SnapPointChecks(targetPosition);
    }
    
    private void HandleInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _targetHeight += (int) Input.mouseScrollDelta.y;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearGhostObject();
            this.enabled = false;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            _ghostObject.transform.Rotate(0, 90, 0);
        }
    }

    /// <summary>
    /// Checks if the ghost object is close enough to a snap point to snap to it
    /// </summary>
    /// <param name="position"></param>
    private void SnapPointChecks(Vector3 position)
    {
        _closestOtherSnapPoint = ClosestPlacedObjectSnapPoint(ClosestPlacedObject(position), position);
        
        if(_closestOtherSnapPoint == null || _ghostObjectScript == null || _ghostObjectScript.GetSnapPoints() == null)
        {
            return;
        }
        _closestOwnSnapPoint = ClosestListObjectToVector(_ghostObjectScript.GetSnapPoints(), _closestOtherSnapPoint.transform.position);
        
        if(_closestOwnSnapPoint == null)
        {
            return;
        }
        
        if (!hasSnapped)
        {
            SetPosition();
        }   
    }

    /// <summary>
    /// Set the position of the ghost object to that of the snap point
    /// </summary>
    private void SetPosition()
    {
        _ghostObject.transform.position = _closestOtherSnapPoint.transform.position;
        Vector3 offset = _ghostObject.transform.position - _closestOwnSnapPoint.transform.position;
        _ghostObject.transform.position += offset;
        _hookedSnapPoint = _closestOtherSnapPoint;
        hasSnapped = true; 
    }

    /// <summary>
    /// Function to set the ghost objects position to that of the mouse (targetPosition)
    /// </summary>
    /// <param name="targetPosition"></param>
    private void ObjectMovement(Vector3 targetPosition)
    {
        if (_ghostObject == null)
        {
            return; 
        }
        
        if (!hasSnapped) //There is free movement if the object has not snapped
        {
            _ghostObject.transform.position = targetPosition;
        }
        else
        {
            if(Vector3.Distance(targetPosition, _hookedSnapPoint.transform.position) > snappingRadius) //If the object is too far away from the snap point, it will be released
            {
                _hookedSnapPoint = null;
                hasSnapped = false;
            }
        }
    }
    
    /// <summary>
    /// Get the closest placed object to the mouse
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private GameObject ClosestPlacedObject(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, snappingRadius);
        List<GameObject> validObjects = new List<GameObject>();

        foreach (Collider collider in hitColliders)
        {
            if (!collider.CompareTag("Selectable")) continue;
            if(collider.gameObject == _ghostObject)
            {
                continue;
            }
            validObjects.Add(collider.gameObject);
        }
        
        return ClosestListObjectToVector(validObjects, position);
    }
    
    /// <summary>
    /// Get the closest snap point of the closest placed object to the mouse
    /// </summary>
    /// <param name="placedObject"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private GameObject ClosestPlacedObjectSnapPoint(GameObject placedObject, Vector3 position)
    {
        if(placedObject == null)
        {
            return null;
        }
        
        List<GameObject> snapPoints = placedObject.GetComponent<PlacedObject>().snappingPointsGenerator.snapPoints;
        return ClosestListObjectToVector(snapPoints, position);
    }

    /// <summary>
    /// Calculates the closest object in a list to a given vector
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="location"></param>
    /// <returns>The GameObject from the list closest to the location</returns>
    private GameObject ClosestListObjectToVector(List<GameObject> objects, Vector3 location)
    {
        if(objects.Count == 0)
        {
            return null;
        }
        
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (GameObject obj in objects)
        {
            if (!(Vector3.Distance(obj.transform.position, location) < closestDistance)) continue;
         
            closestDistance = Vector3.Distance(obj.transform.position, location);
            closestObject = obj;
        }
        
        return closestObject;
    }

    public GameObject GetGhostObject()
    {
        return _ghostObject;
    }

    /// <summary>
    /// Get the position of the mouse on the ground
    /// </summary>
    /// <returns></returns>
    private Vector3 GetTargetPosition()
    {            
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit))
        {
        }

        float t = -ray.origin.y / ray.direction.y;
        Vector3 hitPoint = ray.origin + t * ray.direction;
        hitPoint.y = _targetHeight;
        return hitPoint;
    }
}