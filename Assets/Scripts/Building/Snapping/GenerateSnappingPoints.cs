using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSnappingPoints : MonoBehaviour
{
    /*
     * Snapping points are used for snapping objects to each other in building mode. Each object has 6 snapping points, one on each face.
     * If the object is not a cube or variant of that, we use the center and bounds to draw an imaginary cube around the object and set the points on the center of each face.
     */
    
    public List<GameObject> snapPoints = new List<GameObject>();
    
    private PlacedObject po;
    
    void Start()
    {
        po = GetComponent<PlacedObject>();

        StartCoroutine(GenerateFacePoints());
    }
    
    /// <summary>
    /// Generates Snapping points on the center of each face
    /// </summary>
    private IEnumerator GenerateFacePoints()
    {
        while(po == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        while(po.ObjectName == null || po.ObjectName == "")
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        
        /*
         *  We need to use a temporary gameobject because we need a version with a nullified rotation for correct snapping point generation.
         *  We cannot temporarily set the rotation of own gameobject because clients have no authority over it.
         *  We also need to destroy the script because we don't want to generate snapping points for the temporary gameobject, this could cause an infinite loop.
         */
        
        GameObject temp = Instantiate(BuildingManager.Instance.placeableObjectsDict[po.ObjectName], transform.position, transform.rotation); 
        Destroy(temp.GetComponent<GenerateSnappingPoints>());
        temp.transform.rotation = Quaternion.identity;
        
        Bounds bounds = temp.GetComponent<Renderer>().bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        Vector3[] points = new Vector3[6];

        points[0] = center + new Vector3(0, extents.y, 0); //top
        points[1] = center + new Vector3(0, -extents.y, 0); //bottom
        points[2] = center + new Vector3(extents.x, 0, 0); //right
        points[3] = center + new Vector3(-extents.x, 0, 0); //left
        points[4] = center + new Vector3(0, 0, extents.z); //front
        points[5] = center + new Vector3(0, 0, -extents.z); //back


        int count = 0;
        foreach (Vector3 point in points)
        {
            GameObject snapPoint = null;
            if (BuildingManager.Instance.DebugSnapPoints)
            {
                snapPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere); //This is for debugging purposes
            }
            else
            {
                snapPoint = new GameObject();
            }
            snapPoint.tag = "SnapPoint";
            snapPoint.transform.position = point;
            snapPoint.AddComponent<BoxCollider>();
            snapPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            snapPoint.transform.SetParent(temp.transform);
            
            // Snappoint info is not used for anything at the moment, but it could be used to determine what type of snap point it is, until then it is commented out
            
            // SnapPoint info = snapPoint.AddComponent<SnapPoint>();
            //
            // switch (count)
            // {
            //     case 0:
            //         info.snapPointType = SnapPointType.Top;
            //         break;
            //     
            //     case 1:
            //         info.snapPointType = SnapPointType.Bottom;
            //         break;
            //     
            //     case 2:
            //         info.snapPointType = SnapPointType.Right;
            //         break;
            //     
            //     case 3:
            //         info.snapPointType = SnapPointType.Left;
            //         break;
            //     
            //     case 4:
            //         info.snapPointType = SnapPointType.Front;
            //         break;
            //     
            //     case 5:
            //         info.snapPointType = SnapPointType.Back;
            //         break;
            // }
            
            count++;
            
            
            snapPoints.Add(snapPoint);
        }
        
        temp.transform.rotation = transform.rotation;
        
        foreach(GameObject snapPoint in snapPoints)
        {
            snapPoint.transform.SetParent(transform);
        }

        Destroy(temp);
        
        StopCoroutine(GenerateFacePoints());
        
    }
}
