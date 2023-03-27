using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private string prefabLocation;
    
    [SerializeField] private List<GameObject> placeableObjects;
    [SerializeField] private CustomNetworkManager customNetworkManager;
    
    public Dictionary<string, GameObject> placeableObjectsDict = new Dictionary<string, GameObject>();

    private void Start()
    {
        
        placeableObjects = Resources.LoadAll<GameObject>(prefabLocation).ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            placeableObjectsDict.Add(prefab.name, prefab);
        }
        
        
        //if (customNetworkManager == null)
            //customNetworkManager = FindObjectOfType<CustomNetworkManager>();
        
        if(placeableObjects.Count == 0)
            Debug.LogError("No placeable objects found");
        
        //customNetworkManager.spawnPrefabs = placeableObjects;
        
        
    }
}
