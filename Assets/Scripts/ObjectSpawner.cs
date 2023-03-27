using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private GameObject parent;
    [SerializeField] private Builder builder;
    
    private void Awake()
    {
        if(parent == null)
            parent = new GameObject("Parent");
        
        if(builder == null)
            builder = FindObjectOfType<Builder>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject go = Instantiate(builder.placeableObjectsDict["Cube"]);
            go.transform.position = new Vector3(0, 0, 0);
            go.transform.SetParent(parent.transform);
            go.tag = "Selectable";
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameObject go = Instantiate(builder.placeableObjectsDict["Capsule"]);
            go.transform.SetParent(parent.transform);
            go.transform.position = new Vector3(0, 0, 0);
            go.tag = "Selectable";
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject go = Instantiate(builder.placeableObjectsDict["Sphere"]);
            go.transform.SetParent(parent.transform);
            go.transform.position = new Vector3(0, 0, 0);
            go.tag = "Selectable";
        }
    }
}
