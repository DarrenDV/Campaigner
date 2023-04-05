using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostObject : MonoBehaviour
{
    private GenerateSnappingPoints _generateSnappingPoints;

    private void Start()
    {
        _generateSnappingPoints = gameObject.AddComponent<GenerateSnappingPoints>();

    }

    public List<GameObject> GetSnapPoints()
    {
        return _generateSnappingPoints.snapPoints;
    }
    
}
