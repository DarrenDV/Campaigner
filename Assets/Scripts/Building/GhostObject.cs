using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostObject : MonoBehaviour
{
    private GenerateSnappingPoints _generateSnappingPoints;
    private List<Vector3> _startingSnapPointOffsets = new List<Vector3>();

    [SerializeField] private List<Collider> colliders = new List<Collider>();

    [SerializeField] private GameObject _mySnapPoint;
    [SerializeField] private GameObject _otherSnapPoint;
    [SerializeField] private float snappedDistance;
    
    public bool isSnapped = false;

    private void Start()
    {
        _generateSnappingPoints = gameObject.AddComponent<GenerateSnappingPoints>();
        //SetStartingOffsets();
    }
    
    private void SetStartingOffsets()
    {
        if (_generateSnappingPoints.snapPoints.Count != 0)
        {
            foreach (GameObject snapPoint in _generateSnappingPoints.snapPoints)
            {
                _startingSnapPointOffsets.Add(snapPoint.transform.position - transform.position);
            }
        }
        else
        {
            Invoke(nameof(SetStartingOffsets), 0.1f);
        }

    }

    // private void Update()
    // {
    //     //colliders = GetHitColliders();
    //     _mySnapPoint = null;
    //     _otherSnapPoint = null;
    //     ClosestSnapPointCollision();
    //     
    //     if (_mySnapPoint != null && _otherSnapPoint != null)
    //     {
    //         //SnapToOtherObject();
    //         isSnapped = true;
    //     }
    //     else
    //     {
    //         isSnapped = false;
    //     }
    //     
    // }

    private void SnapToOtherObject()
    {
        Vector3 offset = _otherSnapPoint.transform.position - _mySnapPoint.transform.position;
        transform.position = _otherSnapPoint.transform.position + offset;
    }

    public GameObject GetOwnClosestSnappingPoint(GameObject otherSnapPoint)
    {
        GameObject closestSnapPoint = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (GameObject snapPoint in _generateSnappingPoints.snapPoints)
        {
            float distance = Vector3.Distance(snapPoint.transform.position, otherSnapPoint.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSnapPoint = snapPoint;
            }
        }
        
        return closestSnapPoint;
    }

    public void ResetOwnPosition()
    {
        Vector3 averagePosition = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            averagePosition += child.position - _startingSnapPointOffsets[i];
        }
        averagePosition /= transform.childCount;
        transform.position = averagePosition + _startingSnapPointOffsets[0];
    }
    
    
    public List<GameObject> GetSnapPoints()
    {
        return _generateSnappingPoints.snapPoints;
    }


    // public List<Collider> GetHitColliders()
    // {
    //     List<Collider> AllColliders = new List<Collider>();
    //     List<Collider> hitColliders = new List<Collider>();
    //     
    //     // Get all colliders within snapping radius of each snap point
    //     foreach(GameObject snapPoint in _generateSnappingPoints.snapPoints)
    //     {
    //         Collider[] colliders = Physics.OverlapSphere(snapPoint.transform.position, 3f);
    //         foreach (Collider collider in colliders)
    //         {
    //             if (collider.gameObject.CompareTag("SnapPoint"))
    //             {
    //                 if (collider.gameObject != gameObject)
    //                 {
    //                     AllColliders.Add(collider);
    //                 }
    //             }
    //         }
    //     }
    //     
    //     // Remove all snap points that are already hooked to this object
    //     foreach(Collider collider in AllColliders)
    //     {
    //         bool canAdd = true;
    //         
    //         foreach(GameObject snapPoint in _generateSnappingPoints.snapPoints)
    //         {
    //             if (collider.gameObject == snapPoint)
    //             {
    //                 canAdd = false;
    //             }
    //         }
    //         
    //         if (canAdd)
    //         {
    //             hitColliders.Add(collider);
    //         }
    //     }
    //
    //     return hitColliders;
    // }

    
    
    
    
    
    
    private void ClosestSnapPointCollision()
    {
        GameObject closestSnapPoint = null;
        GameObject mySnapPoint = null;
        float closestDistance = Mathf.Infinity;
        
        foreach(GameObject snapPoint in _generateSnappingPoints.snapPoints)
        {
            // Get all colliders within snapping radius of snap point
            List<Collider> hitColliders = HitColliders(snapPoint.transform.position);
            
            if (hitColliders.Count == 0)
                continue;
            
            // Get the closest snap point
            GameObject closestSnapPointToCurrentPoint = ClosestSnapPoint(hitColliders, snapPoint.transform.position);
            
            if(closestSnapPointToCurrentPoint == null)
                continue;
            
            // Check if this snap point is closer than the current closest snap point
            if (Vector3.Distance(closestSnapPointToCurrentPoint.transform.position, snapPoint.transform.position) < closestDistance)
            {
                if (!isSnapped)
                {
                    closestSnapPoint = closestSnapPointToCurrentPoint;
                    mySnapPoint = snapPoint;
                    closestDistance = Vector3.Distance(closestSnapPointToCurrentPoint.transform.position, snapPoint.transform.position);
                    snappedDistance = closestDistance;
                }
                else
                {
                    if(Vector3.Distance(closestSnapPointToCurrentPoint.transform.position, snapPoint.transform.position) < snappedDistance)
                    {
                        closestSnapPoint = closestSnapPointToCurrentPoint;
                        mySnapPoint = snapPoint;
                        closestDistance = Vector3.Distance(closestSnapPointToCurrentPoint.transform.position, snapPoint.transform.position);
                        snappedDistance = closestDistance;
                    }
                }
                

            }
            
        }
        
        _mySnapPoint = mySnapPoint;
        _otherSnapPoint = closestSnapPoint;


    }

    private List<Collider> HitColliders(Vector3 position)
    {
        List<Collider> hitColliders = Physics.OverlapSphere(position, 3f).ToList();
        List<Collider> snapPointColliders = new List<Collider>();
        List<Collider> otherSnapPointColliders = new List<Collider>();
        
        foreach(Collider collider in hitColliders)
        {
            if (collider.gameObject.CompareTag("SnapPoint"))
            {
                snapPointColliders.Add(collider); //I make an array of only the snap points
            }
        }

        foreach (Collider collider in snapPointColliders)
        {
            bool ownsSnapPoint = false;
            
            foreach (GameObject snapPoint in _generateSnappingPoints.snapPoints)
            {
                if (collider.gameObject == snapPoint)
                {
                    ownsSnapPoint = true;
                }
            }
            
            if (!ownsSnapPoint)
            {
                otherSnapPointColliders.Add(collider);
            }
            
        }

        return otherSnapPointColliders;

    }
    
    
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
    
    private Vector3 GetTargetPosition()
    {            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit))
        {
        }

        float t = -ray.origin.y / ray.direction.y;
        Vector3 hitPoint = ray.origin + t * ray.direction;
        hitPoint.y = 0;
        return hitPoint;
    }
    
    
}
