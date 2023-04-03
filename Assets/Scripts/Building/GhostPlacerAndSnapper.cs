using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostPlacerAndSnapper : MonoBehaviour
{
    private GameObject _ghostObject;
    private Camera mainCam;
    [SerializeField] private int _targetHeight = 0;
    
    public bool hasSnapped = false;
    private GameObject _hookedSnapPoint;
    
    [SerializeField] private float snappingRadius = 10f;
    
    
    
    private GhostObject _ghostObjectScript;

    private void Start()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    /// <summary>
    /// Function called to set the ghost object, this being the prefab that will be placed
    /// </summary>
    /// <param name="ghostObject"></param>
    public void SetGhostObject(GameObject ghostObject)
    {
        _ghostObject = ghostObject;
        _ghostObjectScript = _ghostObject.GetComponent<GhostObject>();
    }
    
    /// <summary>
    /// Reset function to clear the ghost object
    /// </summary>
    public void ClearGhostObject()
    {
        Destroy(_ghostObject);
        _ghostObjectScript = null;
        _ghostObject = null;
        _hookedSnapPoint = null;
        hasSnapped = false;
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
        SnapObject(targetPosition);
    }

    /// <summary>
    /// Function to set the ghost objects position to that of the mouse (targetPosition)
    /// </summary>
    /// <param name="targetPosition"></param>
    private void ObjectMovement(Vector3 targetPosition)
    {
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
    /// Snap the ghost object to the closest snap point
    /// </summary>
    /// <param name="targetPosition"></param>
    private void SnapObject(Vector3 targetPosition)
    {
        List<Collider> hitColliders = GetHitColliders(targetPosition);
        
        if (hitColliders.Count == 0)
        {
            return;
        }
        
        GameObject closestSnapPoint = ClosestSnapPoint(hitColliders, targetPosition);
        
        if (closestSnapPoint != null)
        {
            _hookedSnapPoint = closestSnapPoint;
            // GameObject ownClosestSnapPoint = _ghostObjectScript.GetOwnClosestSnappingPoint(_hookedSnapPoint);
            // ownClosestSnapPoint.transform.position = closestSnapPoint.transform.position;
            // _ghostObjectScript.ResetOwnPosition();
            hasSnapped = true;
            _ghostObject.transform.position = GetPositionWithBounds(_hookedSnapPoint, _ghostObject);
        }
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
    
    public GameObject GetGhostObject()
    {
        return _ghostObject;
    }
    
    /// <summary>
    /// Gets all the colliders that are within the snapping radius 
    /// </summary>
    /// <param name="targetPosition">The mouse position at the base of the sphere</param>
    /// <returns>A list of colliders around the object that'll be placed</returns>
    private List<Collider> GetHitColliders(Vector3 targetPosition) //TODO: FIX THIS MESS
    {
        List<Collider> hitColliders = Physics.OverlapSphere(targetPosition, snappingRadius).ToList();
        List<Collider> snapPointColliders = new List<Collider>();
        List<Collider> otherObjectColliders = new List<Collider>();

        foreach(Collider collider in hitColliders)
        {
            if (collider.gameObject.CompareTag("SnapPoint"))
            {
                snapPointColliders.Add(collider); //I make an array of only the snap points
            }
        }

        foreach (Collider collider in snapPointColliders) //I check if the snap point is one of the ghost objects snap points and discard it if it is
        {
            bool isOwnSnapPoint = false;
            foreach (GameObject go in _ghostObjectScript.GetSnapPoints())
            {
                if (collider.gameObject == go)
                {
                    isOwnSnapPoint = true;
                }
            }
            
            if (!isOwnSnapPoint)
            {
                otherObjectColliders.Add(collider);
            }
        }

        return otherObjectColliders;
    }
    
    /// <summary>
    /// Get the position of the mouse on the ground
    /// </summary>
    /// <returns></returns>
    private Vector3 GetTargetPosition()
    {            
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit))
        {
        }

        float t = -ray.origin.y / ray.direction.y;
        Vector3 hitPoint = ray.origin + t * ray.direction;
        hitPoint.y = _targetHeight;
        return hitPoint;
    }

    /// <summary>
    /// Returns the closest snap point to the target position
    /// </summary>
    /// <param name="hitColliders">All colliders found around the object</param>
    /// <param name="targetPosition">Mouse position</param>
    /// <returns> SnapPoint Gameobject that is closest to the ghost object</returns>
    private GameObject ClosestSnapPoint(List<Collider> hitColliders, Vector3 targetPosition)
    {
        GameObject closestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if(Vector3.Distance(collider.transform.position, targetPosition) < closestDistance)
            {
                closestPoint = collider.gameObject;
                closestDistance = Vector3.Distance(collider.transform.position, targetPosition);
            }
        }
        
        return closestPoint;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    /// <summary>
    /// Get the location used for spawning the object
    /// </summary>
    /// <param name="snapPoint"></param>
    /// <param name="ghostObject"></param>
    /// <returns></returns>
    private Vector3 GetPositionWithBounds(GameObject snapPoint, GameObject ghostObject)
    {
        Vector3 updatedPosition = snapPoint.transform.position; //We start with the snap point position

        SnapPointType snapPointType = snapPoint.GetComponent<SnapPoint>().snapPointType; //We get the snap point type
        Vector3 ghostObjectBoundsExtend = GetBoundsExtends(ghostObject); //We get the bounds extend of the ghost object

        //We add the bounds extend to the position depending on the snap point type
        switch (snapPointType)
        {
            //We dont need this because we normally add the bounds extend to the y position
            // case SnapPointType.Top:
            //     updatedPosition.y += ghostObjectBoundsExtend.y;
            //     break;
            
            case SnapPointType.Bottom:
                //updatedPosition.y -= ghostObjectBoundsExtend.y;
                updatedPosition.y -= ghostObjectBoundsExtend.y * 2; //We double this due to the fact that we normally add the bounds extend to the y position
                break;
            
            case SnapPointType.Right:
                updatedPosition.x += ghostObjectBoundsExtend.x;
                break;
            
            case SnapPointType.Left:
                updatedPosition.x -= ghostObjectBoundsExtend.x;
                break;
            
            case SnapPointType.Front:
                updatedPosition.z += ghostObjectBoundsExtend.z;
                break;
            
            case SnapPointType.Back:
                updatedPosition.z -= ghostObjectBoundsExtend.z;
                break;
        }
        
        updatedPosition.y += ghostObjectBoundsExtend.y;
        

        return updatedPosition;
    }
    
    public Vector3 GetBoundsExtends(GameObject go){
        return go.GetComponent<Renderer>().bounds.extents;
    }
}