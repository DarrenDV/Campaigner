using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;


public class BigBuilder : MonoBehaviour
{
    public static BigBuilder Instance { get; private set; }
    
    [SerializeField] private List<GameObject> placeableObjects;
    public Dictionary<string, GameObject> placeableObjectsDict = new Dictionary<string, GameObject>();
    
    
    private GameObject parent;

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
        
        if(parent == null)
            parent = new GameObject("Parent");
    }

    private void Start()
    {
        placeableObjects = Resources.LoadAll<GameObject>("Prefabs").ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            placeableObjectsDict.Add(prefab.name, prefab);
        }
    }

    public void SpawnObject(string objectName)
    {
        GameObject go = Instantiate(placeableObjectsDict[objectName]);
        go.name = objectName;
        NetworkServer.Spawn(go);
        go.transform.position = new Vector3(0, 0, 0);
        go.transform.SetParent(parent.transform);
        go.tag = "Selectable";
    }
}