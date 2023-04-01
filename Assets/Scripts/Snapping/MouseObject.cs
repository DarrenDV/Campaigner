using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseObject : MonoBehaviour
{
    public GameObject BuildThing;
    public GenerateSnappingPoints buildThingSnappingPoints;
    public int targetHeight = 0;

    public Camera mainCam;
    
    public List<Collider> otherColliders = new List<Collider>();

    public bool updateObjectPosition = true;
    public GameObject hookedSnapPoint;

    public float snappingRadius = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        buildThingSnappingPoints = BuildThing.GetComponent<GenerateSnappingPoints>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = GetTargetPosition();

        if (updateObjectPosition)
        {
            BuildThing.transform.position = targetPosition;
        }
        else
        {
            if(Vector3.Distance(targetPosition, hookedSnapPoint.transform.position) > snappingRadius)
            {
                hookedSnapPoint = null;
                updateObjectPosition = true;
            }
        }
        
        
        
        

        otherColliders.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, snappingRadius);
        
        foreach (Collider collider in hitColliders)
        {
            if(collider.CompareTag("SnapPoint"))
            {
                if (!buildThingSnappingPoints.snapPoints.Contains(collider.gameObject))
                {
                    otherColliders.Add(collider);
                }
            }
        }
        
        if (otherColliders.Count > 0)
        {
            GameObject closestPoint = ClosestSnapPoint(otherColliders, targetPosition);
            if (closestPoint != null)
            {
                hookedSnapPoint = closestPoint;
                updateObjectPosition = false;
                BuildThing.transform.position = GetPositionWithBounds(closestPoint, BuildThing);
                
            }

        }

    }

    private GameObject ClosestSnapPoint(List<Collider> hitColliders, Vector3 targetPosition)
    {
        GameObject closestPoint = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider hitCollider in hitColliders)
        {
            if(Vector3.Distance(hitCollider.transform.position, targetPosition) < closestDistance)
            {
                closestPoint = hitCollider.gameObject;
                closestDistance = Vector3.Distance(hitCollider.transform.position, targetPosition);
            }
        }

        return closestPoint;
    }

    private Vector3 GetTargetPosition()
    {            
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit))
        {
        }

        float t = -ray.origin.y / ray.direction.y;
        Vector3 hitPoint = ray.origin + t * ray.direction;
        hitPoint.y = targetHeight;
        return hitPoint;
    }

    private Vector3 GetPositionWithBounds(GameObject snapPoint, GameObject buildThing)
    {
        Vector3 updatedPosition = snapPoint.transform.position;

        SnapPointType snapPointType = snapPoint.GetComponent<SnapPoint>().snapPointType;
        Vector3 buildThingBoundsExtents = buildThing.GetComponent<Renderer>().bounds.extents;

        switch (snapPointType)
        {
            case SnapPointType.Top:
                updatedPosition.y += buildThingBoundsExtents.y;
                break;
            
            case SnapPointType.Bottom:
                updatedPosition.y -= buildThingBoundsExtents.y;
                break;
            
            case SnapPointType.Right:
                updatedPosition.x += buildThingBoundsExtents.x;
                break;
            
            case SnapPointType.Left:
                updatedPosition.x -= buildThingBoundsExtents.x;
                break;
            
            case SnapPointType.Front:
                updatedPosition.z += buildThingBoundsExtents.z;
                break;
            
            case SnapPointType.Back:
                updatedPosition.z -= buildThingBoundsExtents.z;
                break;
        }
        

        return updatedPosition;
    }


    // private Vector3 GetMouseWorldPosition()
    // {
    //     Vector3 vec = Input.mousePosition;
    //     vec.z = Camera.main.transform.position.y;
    //     vec = Camera.main.ScreenToWorldPoint(vec);
    //     vec.y = 0;
    //     return vec; 
    // }
}
