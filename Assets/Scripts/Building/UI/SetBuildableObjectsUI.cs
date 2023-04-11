using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetBuildableObjectsUI : MonoBehaviour
{
    [SerializeField] private GameObject buildableObjectPrefab;
    
    [SerializeField] private GameObject buildableObjectListContent;
    
    [SerializeField] private List<BuildableObject> buildableObjects = new List<BuildableObject>();
    
    
    private void Start()
    {
        SetUI();
    }

    private void SetUI()
    {
        List<GameObject> placeableObjects;
        
        placeableObjects = Resources.LoadAll<GameObject>($"Prefabs").ToList();

        foreach (GameObject prefab in placeableObjects)
        {
            GameObject buildableObject = Instantiate(buildableObjectPrefab);
            BuildableObject buildableObjectScript = buildableObject.GetComponent<BuildableObject>();
            
            buildableObjectScript.objectName = prefab.name;
            buildableObjectScript.SetObjectImage(prefab);
            
            buildableObject.transform.SetParent(buildableObjectListContent.transform);
            buildableObject.transform.localScale = Vector3.one;
            
            buildableObjects.Add(buildableObjectScript);

        }
    }
    
}
